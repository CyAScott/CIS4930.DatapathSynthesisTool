using System;
using NUnit.Framework;
using Synthesize.FileParsing;

namespace Synthesize.Tests
{
    [TestFixture]
    public class AifFileTests
    {
        public static AifFile BookExample => new AifFile(
            "inputs x 4 u 4 dx 4 y 4 a 4 c3 4",
            "outputs u1 4 y1 4 x1 4 c 4",
            "regs r01 4 r02 4 r03 4 r04 4 r06 4 r07 4 r08 4",
            "op01 MULT 4 c3  x   r01",
            "op02 MULT 4 u   dx  r02",
            "op03 MULT 4 r01 r02 r03",
            "op04 COMP 4 u   r03 r04",
            "op05 COMP 4 r04 r07 u1",
            "op06 MULT 4 c3  y   r06",
            "op07 MULT 4 r06 dx  r07",
            "op08 MULT 4 u   dx  r08",
            "op09 COMP 4 y   r08 y1",
            "op10 COMP 4 x   dx  x1",
            "op11 COMP 4 x1  a   c",
            "end");
        [Test]
        public void TestBookExample()
        {
            var file = BookExample;
            Assert.IsNotNull(file);
            Console.WriteLine($"MinCycles: {file.MinCycles}");
            Console.WriteLine($"Ops: {string.Join(", ", file.OperationTypes)}");
            Console.WriteLine(file);
        }

        public static AifFile Ellip => new AifFile(
            "inputs inp 4 svin2 4 svin13 4 svin18 4 svin26 4 svin33 4 svin38 4 svin39 4 TWO 4",
            "outputs svout2 4 svout13 4 svout18 4 svout26 4 svout33 4 svout38 4 svout39 4",
            "regs outpi 4 var3   4 var32  4 var12  4 var20 4 var25 4 var21 4     var24 4 var19  4 var27 4 var11 4 var22 4 var29 4 var9 4 var30 4 var8   4 var31 4 var7 4 var10 4 var28 4 var41 4 var6 4 var15 4 var35 4 var4 4 var16 4 var36 4",
            "op1 ADD 4 inp svin2 var3",
            "op2 ADD 4 svin33 svin39 var32",
            "op3 ADD 4 var3 svin13  var12",
            "op4 ADD 4 var12 svin26 var20",
            "op5 ADD 4 var20 var32  var25",
            "op6 MULT 4 var25 TWO var21",
            "op7 MULT 4 var25 TWO var24",
            "op8 ADD 4 var12 var21 var19",
            "op9 ADD 4 var24 var32 var27",
            "op10 ADD 4 var12 var19 var11",
            "op11 ADD 4 var19 var25 var22",
            "op12 ADD 4 var27 var32 var29",
            "op13 MULT 4 var11 TWO var9",
            "op14 ADD 4 var22 var27 svout26",
            "op15 MULT 4 var29 TWO var30",
            "op16 ADD 4 var3 var9 var8",
            "op17 ADD 4 var30 svin39 var31",
            "op18 ADD 4 var3 var8 var7",
            "op19 ADD 4 var8 var19 var10",
            "op20 ADD 4 var27 var31 var28",
            "op21 ADD 4 var31 svin39 var41",
            "op22 MULT 4 var7 TWO var6",
            "op23 ADD 4 var10 svin18 var15",
            "op24 ADD 4 svin38 var28 var35",
            "op25 MULT 4 var41 TWO outpi",
            "op26 ADD 4 inp var6 var4",
            "op27 MULT 4 var15 TWO var16",
            "op28 MULT 4 var35 TWO var36",
            "op29 ADD 4 var31 outpi svout39",
            "op30 ADD 4 var4 var8 svout2",
            "op31 ADD 4 var16 svin18 svout18",
            "op32 ADD 4 svin38 var36 svout38",
            "op33 ADD 4 var15 svout18 svout13",
            "op34 ADD 4 svout38 var35 svout33",
            "",
            "end");
        [Test]
        public void TestEllip()
        {
            var file = Ellip;
            Assert.IsNotNull(file);
            Console.WriteLine($"MinCycles: {file.MinCycles}");
            Console.WriteLine($"Ops: {string.Join(", ", file.OperationTypes)}");
            Console.WriteLine(file);
        }

        /// <summary>
        /// yout = f(c1, x0, c2, x1, c3, x2, c4, x3, c5, x4) = (c1 * x0) + (c2 * x1) + (c3 * x2) + (c4 * x3) + (c5 * x4)
        /// </summary>
        public static AifFile Fir => new AifFile(
            "inputs c1 8 x0 8 c2 8 x1 8 c3 8 x2 8 c4 8 x3 8 c5 8 x4 8  ",
            "outputs yout 8",
            "regs r0 8 r1 8 r2 8 r3 8 r4 8 r5 8 r6 8 r7 8 ",
            "op1 MULT 8 c1 x0 r0",
            "op2 MULT 8 c2 x1 r1",
            "op3 ADD 8 r0 r1 r2 ",
            "op4 MULT 8 c3 x2 r3",
            "op5 ADD 8 r2 r3 r4 ",
            "op6 MULT 8 c4 x3 r5",
            "op7 ADD 8 r4 r5 r6 ",
            "op8 MULT 8 c5 x4 r7",
            "op9 ADD 8 r6 r7 yout",
            "end ",
            "");
        [Test]
        public void TestFir()
        {
            var file = Fir;
            Assert.IsNotNull(file);
            Console.WriteLine($"MinCycles: {file.MinCycles}");
            Console.WriteLine($"Ops: {string.Join(", ", file.OperationTypes)}");
            Console.WriteLine(file);
        }

        public static AifFile Iir => new AifFile(
            "inputs a1 8 y1 8 a2 8 y2 8 b0 8 x0 8 b1 8 x1 8 b2 8 x2 8 ",
            "outputs yout 8",
            "regs r1 8 r2 8 r3 8 r4 8 r5 8 r6 8 r7 8 r8 8 ",
            "op1 MULT 8 a1 y1 r1",
            "op2 MULT 8 a2 y2 r2 ",
            "op3 MULT 8 b0 x0 r3",
            "op4 MULT 8 b1 x1 r4 ",
            "op5 MULT 8 b2 x2 r5",
            "op6 ADD 8 r1 r2 r6",
            "op7 ADD 8 r3 r4 r7",
            "op8 ADD 8 r7 r5 r8",
            "op9 ADD 8 r6 r8 yout",
            "end",
            "",
            "",
            "");
        [Test]
        public void TestIir()
        {
            var file = Iir;
            Assert.IsNotNull(file);
            Console.WriteLine($"MinCycles: {file.MinCycles}");
            Console.WriteLine($"Ops: {string.Join(", ", file.OperationTypes)}");
            Console.WriteLine(file);
        }

        public static AifFile InputExample => new AifFile(
            "inputs a 4 b 4 c 4 d 4 e 4 f 4 g 4 h 4",
            "outputs i 4",
            "regs    t0 4  t1 4  t2 4  t3 4  t4 4  t5 4 ",
            "op1 MULT 4 a b t0",
            "op2 MULT 4 c d t1",
            "op3 MULT 4 e f t2",
            "op4 MULT 4 t0 t1 t3",
            "op5 MULT 4 g t2 t4",
            "op6 SUB  4 t3 h t5",
            "op7 SUB  4 t5 t4 i",
            "end");
        [Test]
        public void TestInputExample()
        {
            var file = InputExample;
            Assert.IsNotNull(file);
            Console.WriteLine($"MinCycles: {file.MinCycles}");
            Console.WriteLine($"Ops: {string.Join(", ", file.OperationTypes)}");
            Console.WriteLine(file);
        }

        public static AifFile Lattice => new AifFile(
            "inputs x 8 px0 8 px1 8 c1 8 c2 8 c3 8 c4 8 c5 8",
            "outputs y 8 x0 8 x1 8",
            "regs r0 8 r1 8 r2 8 r3 8 r4 8 r5 8 r6 8 r7 8 r8 8 r9 8",
            "op1 ADD 8 x px0 r0",
            "op2 MULT 8 c1 r0 r1",
            "op3 ADD 8 x r1 r2",
            "op4 ADD 8 r2 px1 r3",
            "op5 MULT 8 c2 r3 r4",
            "op6 ADD 8 r2 r4 x1",
            "op7 ADD 8 r4 x1 x0",
            "op8 ADD 8 r1 x0 r5",
            "op9 MULT 8 c3 r5 r6",
            "op10 MULT 8 c5 x1 r7",
            "op11 MULT 8 c4 x0 r8",
            "op12 ADD 8 r7 r8 r9",
            "op13 ADD 8 r9 r6 y",
            "end ",
            "",
            "");
        [Test]
        public void TestLattice()
        {
            var file = Lattice;
            Assert.IsNotNull(file);
            Console.WriteLine($"MinCycles: {file.MinCycles}");
            Console.WriteLine($"Ops: {string.Join(", ", file.OperationTypes)}");
            Console.WriteLine(file);
        }
        public static AifFile ToyExample => new AifFile(
            "inputs a 4 b 4 c 4 d 4 e 4 f 4 g 4 h 4",
            "outputs i 4",
            "regs    t0 4  t1 4  t2 4  t3 4  t4 4  t5 4",
            "op1 MULT 4 a b t0",
            "op2 MULT 4 c d t1",
            "op3 MULT 4 e f t2",
            "op4 MULT 4 t0 t1 t3",
            "op5 MULT 4 g t2 t4",
            "op6 SUB  4 t3 h t5",
            "op7 SUB  4 t5 t4 i",
            "end",
            "");

        [Test]
        public void TestToyExample()
        {
            var file = ToyExample;
            Assert.IsNotNull(file);
            Console.WriteLine($"MinCycles: {file.MinCycles}");
            Console.WriteLine($"Ops: {string.Join(", ", file.OperationTypes)}");
            Console.WriteLine(file);
        }
    }
}
