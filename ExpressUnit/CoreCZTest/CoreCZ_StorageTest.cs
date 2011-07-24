using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressUnitModel;
using CoreCZ;

namespace CoreCZTest
{
    //[TestClass]
    public class CoreCZ_StorageTest
    {
        [UnitTest]
        public void StorageCreate()
        {
            Storage<int> s = new Storage<int>(20, -6);

            Confirm.Equal(s[1, 3], -6);
            Confirm.Equal(s[11, 1], -6);
            Confirm.Equal(s[-1, 13], -6);
            Confirm.Equal(s[0, 0], -6);
            Confirm.Equal(s[-1, 0], -6);
            Confirm.Equal(s[0, 3], -6);
            Confirm.Equal(s[-4, -4], -6);
            Confirm.Equal(s[-2, 0], -6);
            Confirm.Equal(s[0, -18], -6);
        }

        [UnitTest]
        public void StorageSetAndGetValue()
        {
            Storage<int> s = new Storage<int>(20);

            s[0, 0] = 10;
            Confirm.Equal(s[0, 0], 10);

            s[1, 1] = 14;
            Confirm.Equal(s[1, 1], 14);

            s[7, -1] = 1342;
            Confirm.Equal(s[7, -1], 1342);

            s[-3, 0] = 1342;
            Confirm.Equal(s[-3, 0], 1342);

            s[-3, -8] = 1342;
            Confirm.Equal(s[-3, 0], 1342);
        }

        [UnitTest]
        public void StorageSetAndGetElementsOnBorder()
        {
            Storage<int> s = new Storage<int>(10);
           
            s[-10, -10] = 17;
            Confirm.Equal(s[-10, -10], 17);

            s[9, 9] = 18;
            Confirm.Equal(s[9, 9], 18);

            s[0, 9] = 19;
            Confirm.Equal(s[0, 9], 19);

            s[6, 9] = 20;
            Confirm.Equal(s[6, 9], 20);

            s[-10, 5] = 21;
            Confirm.Equal(s[-10, 5], 21);
        }
    }
}
