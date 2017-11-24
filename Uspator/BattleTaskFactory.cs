using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Net.Http.Headers;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Uspator.Model;

namespace Uspator
{
    public class BattleTaskFactory
    {
        private readonly ServerRequest _request;
        private List<string> _forbiddenPositions;
        private Player _virtualMe;
        
        public BattleTaskFactory(ServerRequest request)
        {
            _request = request;
            _forbiddenPositions = new List<string>();
            _virtualMe = null;
        }
        
        public List<TaskBase> GetTaskTypes(int taskCount)
        {
            var tasks = new List<TaskBase>();

            var r = new Random();
            
            for (var i = 0; i < taskCount; i++)
            {
                if (i == 0)
                {
                    tasks.Add(new MoveTask());
                }
                else if (i == 1 && taskCount > 2)
                {
                    tasks.Add(new MoveTask());
                }
                else
                {
                    tasks.Add(new BombTask());
                }
            }
            
            // Make sure there is at least one move-action if more than 1 task
            var noMove = tasks.Any(t => t.GetType() != typeof(MoveTask)) && tasks.Count > 1;
            if (noMove)
            {
                tasks[0] = new MoveTask();
            }
            
            return tasks;
        }

        public void ApplyTask(TaskBase task)
        {
            if (task.GetType() == typeof(MoveTask))
            {
                if (!ApplyTask((MoveTask) task))
                {
                    task = new NoopTask();
                }
            }
            else if (task.GetType() == typeof(BombTask))
            {
                if (!ApplyTask((BombTask) task))
                {
                    task = new NoopTask();
                }
            }
        }
        
        private bool ApplyTask(MoveTask task)
        {
            Player self;
            if (_virtualMe == null)
            {
                self = _request.Players.First(p => p.name.ToLowerInvariant()
                    .Equals(Settings.BOTNAME.ToLowerInvariant()));
            }
            else
            {
                Console.WriteLine("Virtual me found");
                self = _virtualMe;
            }
            
            var closestProximity =  _request.Players.Where(p => !p.name.ToLowerInvariant()
                .Equals(Settings.BOTNAME.ToLowerInvariant())).Min(p => p.GetProximity(self));
            var closestPlayer = _request.Players.FirstOrDefault(p => p.GetProximity(self) == closestProximity);

            var axis = closestPlayer.GetMostFarAxis(self);

            var possibleMoves = ListPossibleMoves(self);
            var counter = 0;
            do
            {
                if (counter == Settings.FIND_A_SOLUTION_LIMIT)
                {
                    if (possibleMoves.Count == 0)
                    {
                        return false;
                    }
                    var move = possibleMoves.First();
                    task.Direction = self.FindPositionDelta(move);
                    break;
                }

                switch (axis)
                {
                    case "X":
                        task.Direction = closestPlayer.X - self.X > 0 ? "-X" : "+X";
                        break;
                    case "Y":
                        task.Direction = closestPlayer.Y - self.Y > 0 ? "-Y" : "+Y";
                        break;
                    case "Z":
                        task.Direction = closestPlayer.Z - self.Z > 0 ? "-Z" : "+Z";
                        break;
                    default:
                        break;
                }
                counter++;
            } while (!ValidateFuturePosition(self));

            _virtualMe = self.ApplyMove(task.Direction);
            _forbiddenPositions.Add(_virtualMe.GetPositionString());
            
            Console.WriteLine($"Moving to {_virtualMe.GetPositionString()}");
            
            return true;
        }

        private List<string> ListPossibleMoves(Player self)
        {
            var moves = new List<string>();
            var futureMe = self.ApplyMove("+X");
            if (ValidateFuturePosition(futureMe)) moves.Add($"{futureMe.X}{futureMe.Y}{futureMe.Z}"); 
            
            futureMe = self.ApplyMove("-X");
            if (ValidateFuturePosition(futureMe)) moves.Add($"{futureMe.X}{futureMe.Y}{futureMe.Z}"); 
            
            futureMe = self.ApplyMove("+Y");
            if (ValidateFuturePosition(futureMe)) moves.Add($"{futureMe.X}{futureMe.Y}{futureMe.Z}"); 
            
            futureMe = self.ApplyMove("-Y");
            if (ValidateFuturePosition(futureMe)) moves.Add($"{futureMe.X}{futureMe.Y}{futureMe.Z}"); 
            
            futureMe = self.ApplyMove("+Z");
            if (ValidateFuturePosition(futureMe)) moves.Add($"{futureMe.X}{futureMe.Y}{futureMe.Z}"); 
            
            futureMe = self.ApplyMove("-Z");
            if (ValidateFuturePosition(futureMe)) moves.Add($"{futureMe.X}{futureMe.Y}{futureMe.Z}");

            return moves;
        }

        private bool ValidateFuturePosition(Player futureMe)
        {
            if (futureMe.X < 0 || 
                futureMe.X > _request.GameInfo.EdgeLength - 1 ||
                futureMe.Y < 0 || 
                futureMe.Y > _request.GameInfo.EdgeLength - 1 ||
                futureMe.Z < 0 || 
                futureMe.Z > _request.GameInfo.EdgeLength - 1)
            {
                return false;
            }

            if (_request.Players.Any(p => p.GetPositionString() == futureMe.GetPositionString()))
            {
                return false;
            }

            if (_request.Items.Any(i => i.GetPositionString() == futureMe.GetPositionString()))
            {
                return false;
            }

            if (_forbiddenPositions.Contains(futureMe.GetPositionString()))
            {
                return false;
            }
            
            return true;
        }

        private string GetNextAxis(string axis)
        {
            var index = Settings.AXIS.IndexOf(axis, StringComparison.Ordinal);
            return index == 2 ? 
                Settings.AXIS[0].ToString() : 
                Settings.AXIS[index + 1].ToString();
        }

        private bool ApplyTask(BombTask task)
        {
            var target = _request.Players.FirstOrDefault(p => !p.name.ToLowerInvariant()
                .Equals(Settings.BOTNAME.ToLowerInvariant()));
            
            var axis = "X";
            var position = $"{target.X}{target.Y}{target.Z}";
            var targetRange = ListPossibleMoves(target);

            foreach (var s in targetRange)
            {
                position = s;
                if (ValidateBombPosition(position))
                {
                    break;
                }
            }

            task.X = Convert.ToInt16(position[0].ToString());
            task.Y = Convert.ToInt16(position[1].ToString());
            task.Z = Convert.ToInt16(position[2].ToString());
            
            _forbiddenPositions.Add($"{task.X}{task.Y}{task.Z}");
            return true;
        }

        private bool ValidateBombPosition(string position)
        {
            if (position.Contains("8") || position.Contains("-1"))
            {
                return false;
            }
            
            if (_request.Players.Where(p => p.name.ToLowerInvariant().Equals(Settings.BOTNAME.ToLowerInvariant()))
                .Any(p => p.GetPositionString() == position))
            {
                return false;
            }

            if (_request.Items.Any(i => i.GetPositionString() == position))
            {
                return false;
            }
            
            if (_forbiddenPositions.Contains(position))
            {
                return false;
            }

            return true;
        }
    }
}