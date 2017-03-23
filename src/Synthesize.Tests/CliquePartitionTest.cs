using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Synthesize.CliquePartition;

namespace Synthesize.Tests
{
    [TestFixture]
    public class CliquePartitionTest
    {
        [Test]
        public void Test()
        {
            var compat = new int[9][];
            for (var index = 0; index < compat.Length; index++)
            {
                compat[index] = new int[compat.Length];
            }
            
            compat[0][0] = 1; compat[0][1] = 0; compat[0][2] = 0;
            compat[0][3] = 1; compat[0][4] = 0; compat[0][5] = 1;
            compat[0][6] = 0; compat[0][7] = 1; compat[0][8] = 0;

            compat[1][0] = 0; compat[1][1] = 1; compat[1][2] = 0;
            compat[1][3] = 1; compat[1][4] = 0; compat[1][5] = 1;
            compat[1][6] = 0; compat[1][7] = 1; compat[1][8] = 0;

            compat[2][0] = 0; compat[2][1] = 0; compat[2][2] = 1;
            compat[2][3] = 0; compat[2][4] = 0; compat[2][5] = 0;
            compat[2][6] = 0; compat[2][7] = 0; compat[2][8] = 0;

            compat[3][0] = 1; compat[3][1] = 1; compat[3][2] = 0;
            compat[3][3] = 1; compat[3][4] = 0; compat[3][5] = 0;
            compat[3][6] = 0; compat[3][7] = 1; compat[3][8] = 0;

            compat[4][0] = 0; compat[4][1] = 0; compat[4][2] = 0;
            compat[4][3] = 0; compat[4][4] = 1; compat[4][5] = 0;
            compat[4][6] = 0; compat[4][7] = 0; compat[4][8] = 0;

            compat[5][0] = 1; compat[5][1] = 1; compat[5][2] = 0;
            compat[5][3] = 0; compat[5][4] = 0; compat[5][5] = 1;
            compat[5][6] = 0; compat[5][7] = 1; compat[5][8] = 0;

            compat[6][0] = 0; compat[6][1] = 0; compat[6][2] = 0;
            compat[6][3] = 0; compat[6][4] = 0; compat[6][5] = 0;
            compat[6][6] = 1; compat[6][7] = 0; compat[6][8] = 0;

            compat[7][0] = 1; compat[7][1] = 1; compat[7][2] = 0;
            compat[7][3] = 1; compat[7][4] = 0; compat[7][5] = 1;
            compat[7][6] = 0; compat[7][7] = 1; compat[7][8] = 0;

            compat[8][0] = 0; compat[8][1] = 0; compat[8][2] = 0;
            compat[8][3] = 0; compat[8][4] = 0; compat[8][5] = 0;
            compat[8][6] = 0; compat[8][7] = 0; compat[8][8] = 1;

            var helper = new CliqueHelper();

            helper.cliquePartition(compat, compat.Length);
        }
    }
}
