using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ExpressUnitModel;
using CoreCZ;
using CoreCZ.AI;


namespace ExpressUnitGui.CoreCZTest
{
    [TestClass]   
    class CoreCZ_StateTest
    {
        [UnitTest]
        public void State_InitialStateTest()
        {
            GameState gs = new GameState();

            Position pos = new Position(3,5);

            gs.Advance(pos, Side.Cross);

            Confirm.Equal(9, gs.Count<Position>());
            Confirm.Equal(Side.Cross, gs[pos].Side);                                
        }


        [UnitTest]
        public void State_CorrectDirectionDeltas()
        {
            Position pos;

            pos = PositionInfo.GetDirectionVector(PositionInfo.Orientation.Vertical, PositionInfo.Direction.Positive);
            Confirm.Equal(0, pos.X);
            Confirm.Equal(1, pos.Y);

            pos = PositionInfo.GetDirectionVector(PositionInfo.Orientation.Vertical, PositionInfo.Direction.Negative);
            Confirm.Equal(0, pos.X);
            Confirm.Equal(-1, pos.Y);

            pos = PositionInfo.GetDirectionVector(PositionInfo.Orientation.Horizontal, PositionInfo.Direction.Positive);
            Confirm.Equal(1, pos.X);
            Confirm.Equal(0, pos.Y);

            pos = PositionInfo.GetDirectionVector(PositionInfo.Orientation.Horizontal, PositionInfo.Direction.Negative);
            Confirm.Equal(-1, pos.X);
            Confirm.Equal(0, pos.Y);

            pos = PositionInfo.GetDirectionVector(PositionInfo.Orientation.DigonalSW, PositionInfo.Direction.Positive);
            Confirm.Equal(1, pos.X);
            Confirm.Equal(1, pos.Y);

            pos = PositionInfo.GetDirectionVector(PositionInfo.Orientation.DigonalSW, PositionInfo.Direction.Negative);
            Confirm.Equal(-1, pos.X);
            Confirm.Equal(-1, pos.Y);

            pos = PositionInfo.GetDirectionVector(PositionInfo.Orientation.DiagonalNW, PositionInfo.Direction.Positive);
            Confirm.Equal(1, pos.X);
            Confirm.Equal(-1, pos.Y);

            pos = PositionInfo.GetDirectionVector(PositionInfo.Orientation.DiagonalNW, PositionInfo.Direction.Negative);
            Confirm.Equal(-1, pos.X);
            Confirm.Equal(1, pos.Y);                
        
        }

        [UnitTest]
        public void State_DeriveState_OpenFlag()
        {
            //Test case:
            // | | |o|
            // | |x| | --> initial pos(3,3)
            // | | | |

            GameState gs = new GameState();

            Position pos0 = new Position(3, 3);
            Position pos1 = new Position(4, 4);

            gs.Advance(pos0, Side.Cross);
            gs.Advance(pos1, Side.Zero);

            //Sanity:
            Confirm.Equal(Side.Cross, gs[pos0].Side);
            Confirm.Equal(Side.Zero, gs[pos1].Side);
                        
            // (2, 2)
            {
                PositionInfo info = gs[new Position(2, 2)];
                Confirm.IsNotNull(info);
                PositionInfo.LineInfo l = info[PositionInfo.Orientation.DigonalSW, PositionInfo.Direction.Positive];
                Confirm.Equal(Side.Cross, l.side);
                Confirm.Equal(1, (int)l.amount);
                Confirm.Equal(false, l.open);
            }
            // (5, 5)
            {
                PositionInfo info = gs[new Position(5, 5)];
                Confirm.IsNotNull(info);
                PositionInfo.LineInfo l = info[PositionInfo.Orientation.DigonalSW, PositionInfo.Direction.Negative];
                Confirm.Equal(Side.Zero, l.side);
                Confirm.Equal(1, (int)l.amount);
                Confirm.Equal(false, l.open);
            }
        }

        [UnitTest]
        public void State_DeriveStateUpdatesPositionInformation()
        {
            //Test case:
            // | | |o|
            // | |x| | --> initia pos(3,3)
            // | | |x|
            GameState gs = new GameState();
            Position pos0 = new Position(3, 3);
            Position pos1 = new Position(4, 4);
            Position pos2 = new Position(4, 2);
            // Create states
            gs.Advance(pos0, Side.Cross);
            gs.Advance(pos1, Side.Zero);
            gs.Advance(pos2, Side.Cross);
            // (4, 3)
            {
                PositionInfo info = gs[new Position(4, 3)];
                Confirm.IsNotNull(info);
                PositionInfo.LineInfo up = info[PositionInfo.Orientation.Vertical, PositionInfo.Direction.Positive];
                Confirm.Equal(Side.Zero, up.side);
                Confirm.Equal(1, (int)up.amount);
                Confirm.Equal(true, up.open);

                PositionInfo.LineInfo left = info[PositionInfo.Orientation.Horizontal, PositionInfo.Direction.Negative];
                Confirm.Equal(Side.Cross, left.side);
                Confirm.Equal(1, (int)left.amount);
                Confirm.Equal(true, left.open);

                PositionInfo.LineInfo down = info[PositionInfo.Orientation.Vertical, PositionInfo.Direction.Negative];
                Confirm.Equal(Side.Cross, down.side);
                Confirm.Equal(1, (int)down.amount);
                Confirm.Equal(true, down.open);
            }

            // (2,2)
            {
                PositionInfo info = gs[new Position(2, 2)];
                Confirm.IsNotNull(info);
                PositionInfo.LineInfo up = info[PositionInfo.Orientation.DigonalSW, PositionInfo.Direction.Positive];
                Confirm.Equal(Side.Cross, up.side);
                Confirm.Equal(1, (int)up.amount);
                Confirm.Equal(false, up.open);
            }
            // (2,4)
            {
                PositionInfo info = gs[new Position(2, 4)];
                Confirm.IsNotNull(info);
                PositionInfo.LineInfo up = info[PositionInfo.Orientation.DiagonalNW, PositionInfo.Direction.Positive];
                Confirm.Equal(Side.Cross, up.side);
                Confirm.Equal(2, (int)up.amount);
                Confirm.Equal(true, up.open);
            }
            // (5,1)
            {
                PositionInfo info = gs[new Position(5, 1)];
                Confirm.IsNotNull(info);
                PositionInfo.LineInfo up = info[PositionInfo.Orientation.DiagonalNW, PositionInfo.Direction.Negative];
                Confirm.Equal(Side.Cross, up.side);
                Confirm.Equal(2, (int)up.amount);
                Confirm.Equal(true, up.open);
            }
        }

        [UnitTest]
        public void State_DeriveStateUpdatesAlongTheLine()
        {
            //Test case:
            // | | |o| |
            // | |x|o| | --> initia pos(3,3)
            // | | |x| |
            // | | | |x|

            Position pos0 = new Position(3, 3);
            Position pos1 = new Position(4, 4);
            Position pos2 = new Position(4, 2);
            Position pos3 = new Position(3, 2);
            Position pos4 = new Position(5, 1);

            // Create states
            GameState gs = new GameState();
            gs.Advance(pos0, Side.Cross);
            gs.Advance(pos1, Side.Zero);
            gs.Advance(pos2, Side.Cross);
            gs.Advance(pos3, Side.Zero);
            gs.Advance(pos4, Side.Cross);

            //(6,0)
            {
                PositionInfo info = gs[new Position(6, 0)];
                Confirm.IsNotNull(info);
                PositionInfo.LineInfo up = info[PositionInfo.Orientation.DiagonalNW, PositionInfo.Direction.Negative];
                Confirm.Equal(Side.Cross, up.side);
                Confirm.Equal(3, (int)up.amount);
                Confirm.Equal(true, up.open);
            }

            //(2,4)
            {
                PositionInfo info = gs[new Position(2, 4)];
                Confirm.IsNotNull(info);
                PositionInfo.LineInfo up = info[PositionInfo.Orientation.DiagonalNW, PositionInfo.Direction.Positive];
                Confirm.Equal(Side.Cross, up.side);
                Confirm.Equal(3, (int)up.amount);
                Confirm.Equal(true, up.open);
            }

        }

        [UnitTest]
        public void State_DeriveStateAddsOneTurn()
        {
            //Test case:
            // | |.|.|.|            
            // |.|.|o|.|
            // |.|x|.|.| --> initia pos(3,5)
            // |.|.|.| |
            // | | | | |

            GameState gs = new GameState();
            Position pos = new Position(3, 5);
            gs.Advance(pos, Side.Cross);            
            Position turnPos = new Position(4, 4);
            gs.Advance(turnPos, Side.Zero);

            //Confirm.Equal(Side.Cross, gs.Player);
            Confirm.Equal(14, gs.Count<Position>());
            Confirm.Equal(Side.Cross, gs[pos].Side);
            Confirm.Equal(Side.Zero, gs[turnPos].Side);
        }

        [UnitTest]
        public void State_InitialStateHasCorrectPositionInfo()
        {
            GameState gs = new GameState();
            //empty state
            Position pos0 = new Position(4, 5);
            gs.Advance(pos0, Side.Cross);
            
            //
            foreach (PositionInfo.Orientation o in PositionInfo.Orientations)
            {
                foreach (PositionInfo.Direction d in PositionInfo.Directions)
                {
                    Position p = pos0 + PositionInfo.GetDirectionVector(o, d);
                    Confirm.IsNotNull(gs[p]);
                    Confirm.Equal(gs[p].Side, Side.Nobody);

                    PositionInfo info = gs[p];
                    //Check that line is dectected in the direction to pos0
                    //and that all others direction are empty
                    foreach (PositionInfo.Orientation o1 in PositionInfo.Orientations)
                    {
                        foreach (PositionInfo.Direction d1 in PositionInfo.Directions)
                        {
                            PositionInfo.LineInfo l = info[o1, d1];

                            if (o1 == o && d1 != d)
                            {
                                Confirm.Equal(Side.Cross, l.side);
                                Confirm.Equal(1, (int)l.amount);
                                Confirm.Equal(true, l.open);
                            }
                            else
                            {                                
                                Confirm.Equal(0, (int)l.amount);
                            }
                        }
                    }
                }
            }            
        }

        private int getMaxLine(GameState state)
        {
            int max = int.MinValue;
            foreach (Position p in state)
            {
                PositionInfo info = state[p];
                if (info == null || info.Side != Side.Nobody)
                {
                    continue;
                }

                foreach (PositionInfo.Orientation o in PositionInfo.Orientations)
                {
                    foreach (PositionInfo.Direction d in PositionInfo.Directions)
                    {
                        PositionInfo.LineInfo li = info[o, d];
                        if (li.amount > max) max = li.amount;
                    }
                }
            }
            return max;
        }


        void checkUndoRedo(GameState gs, Position p, Side side)
        {
            string before = gs.ToString();
            GameState.ChangeSet change = gs.Advance(p, side);
            string after = gs.ToString();

            Console.WriteLine("BEFORE:");
            Console.WriteLine(before);

            Console.WriteLine("AFTER:");
            Console.WriteLine(after);

            Confirm.Different(before, after);
            gs.Undo(change);
            Console.WriteLine("UNDO:");
            Console.WriteLine(gs);

            Confirm.Equal(before, gs.ToString());
            gs.Advance(p, side);
            Confirm.Equal(after, gs.ToString());           
        }

        [UnitTest]
        public void State_GameStateCheckUndo()
        {
            GameState gs = new GameState();
            Position[] posX = { new Position(5,5), new Position(5,6), new Position(5,7), new Position(4,7) };
            Position[] posO = { new Position(4, 5), new Position(4, 6), new Position(3, 5) };
            
            checkUndoRedo(gs, posX[0], Side.Cross);
            checkUndoRedo(gs, posO[0], Side.Zero);

            checkUndoRedo(gs, posX[1], Side.Cross);
            checkUndoRedo(gs, posO[1], Side.Zero);
        
            checkUndoRedo(gs, posX[2], Side.Cross);
            checkUndoRedo(gs, posO[2], Side.Zero);
        
            checkUndoRedo(gs, posX[3], Side.Cross);
        
        }        
    }
}
