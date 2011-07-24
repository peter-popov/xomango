using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressUnitModel;
using CoreCZ;

namespace CoreCZTest
{
    //[TestClass]
    public class CoreCZ_BoardTest
    {
        [UnitTest]
        public void BoardSetAndGetPosAtDifferentQuadrants()
        {
            
            List<Position> testPositions = new List<Position>() {
                                               new Position(0, 0),
                                               new Position(0, 6),
                                               new Position(5, 12),
                                               new Position(7, 0),
                                               new Position(0, -3),
                                               new Position(-2, -4),
                                               new Position(-8, 0),
                                               new Position(-6, 4),
                                               new Position(4, -6)
                                           };

            foreach(Position p in testPositions)
            {
                Board b = new Board();
                b[p] = Side.Cross;
                Confirm.Equal(b[p], Side.Cross);
            }
        }

        [UnitTest]
        [ExceptionThrown(typeof(InvalidTurnException))]
        public void BoardNoTurnsAtAlreadyOccupiedPosition()
        {
            Board b = new Board();
            Position p = new Position(2, 6);
            b[p] = Side.Cross;
            b[p] = Side.Cross;
        }

        [UnitTest]
        [ExceptionThrown(typeof(InvalidTurnException))]
        public void BoardNoTurnsAtAlreadyOccupiedPositionByEnemy()
        {
            Board b = new Board();
            Position p = new Position(9, 3);
            b[p] = Side.Cross;
            b[p] = Side.Zero;
        }

        [UnitTest]
        public void BoardGetBoundingBoxEmptyBoard()
        {
            Board b = new Board();

            Area a = b.BoundingBox;
            Confirm.IsTrue(a.sw.row > a.ne.row);
            Confirm.IsTrue(a.sw.column > a.ne.column);
        }

        [UnitTest]
        public void BoardGetBoundingBox()
        {
            Board b = new Board();
            b[new Position(0, 0)] = Side.Cross;
            b[new Position(4, 2)] = Side.Zero;
            b[new Position(-2, 2)] = Side.Cross;
            b[new Position(0, 6)] = Side.Zero;
            b[new Position(-1, -1)] = Side.Cross;

            Area a = b.BoundingBox;
            Confirm.IsTrue(a.sw.row == -2);
            Confirm.IsTrue(a.sw.column == -1);
            Confirm.IsTrue(a.ne.row == 4);
            Confirm.IsTrue(a.ne.column == 6);
        }

        [UnitTest]
        public void BoardLastPosition()
        {
            Board b = new Board();
            Position p = new Position(0, 0);
            b[p] = Side.Cross;
            Confirm.Equal(b.LastPosition, p);
            
            p = new Position(4, 2);
            b[p] = Side.Zero;
            Confirm.Equal(b.LastPosition, p);

            p = new Position(-4, 3);
            b[p] = Side.Cross;
            Confirm.Equal(b.LastPosition, p);

            p = new Position(5, -21);
            b[p] = Side.Zero;
            Confirm.Equal(b.LastPosition, p);

            p = new Position(0, 2);
            b[p] = Side.Cross;
            Confirm.Equal(b.LastPosition, p);
        }

        [UnitTest]
        public void BoardUndo()
        {
            Board b = new Board();
            Position p0 = new Position(0, 0);
            b[p0] = Side.Cross;

            Position p1 = new Position(4, 2);
            b[p1] = Side.Zero;

            Position p2 = new Position(-4, 3);
            b[p2] = Side.Cross;

            Position p3 = new Position(5, -21);
            b[p3] = Side.Zero;

            Position p4 = new Position(0, 2);
            b[p4] = Side.Cross;

            Confirm.IsTrue(b.Undo());
            Confirm.Equal(Side.Nobody, b[p4]);

            Confirm.IsTrue(b.Undo());
            Confirm.Equal(Side.Nobody, b[p3]);

            Confirm.IsTrue(b.Undo());
            Confirm.Equal(Side.Nobody, b[p2]);

            Confirm.IsTrue(b.Undo());
            Confirm.Equal(Side.Nobody, b[p1]);

            Confirm.IsTrue(b.Undo());
            Confirm.Equal(Side.Nobody, b[p0]);

            Confirm.IsFalse(b.Undo());
        }

    }
}
