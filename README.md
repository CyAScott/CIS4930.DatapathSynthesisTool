# CIS4930.DatapathSynthesisTool

This is a VHDL synthesis tool for data paths implemented in C#. To build you need to clone the repo and build with Visual Studio 2015 or later. For a demo, follow the tutorial below.

1. When you run the program you will be first prompted to provide an AIF file (the docs\sample\input.aif file is provided in this repo). The program will parse the file and print out the parsed contents. 

```
Provide a path to a AIF file. Enter nothing to exit.
docs\sample\input.aif
inputs a 4 b 4 c 4 d 4 e 4 f 4 g 4 h 4
outputs i 4
regs t0 4 t1 4 t2 4 t3 4 t4 4 t5 4
op1        MULT  04 a          b          t0
op2        MULT  04 c          d          t1
op3        MULT  04 e          f          t2
op4        MULT  04 t0         t1         t3
op5        MULT  04 g          t2         t4
op6        SUB   04 t3         h          t5
op7        SUB   04 t5         t4         i
end
```
2. Next you will be prompted to select a scheduling algorithm for scheduling each function.
```
Select a scheduler algorithm:
        A) List Scheduler (resource constraints required)
        B) Integer Linear Programing Scheduler (time constraint required)
        C) Exit
b
```
3. Choose the number of clock cycles this data path should take.
```
Enter a latency constraint greater then or equal to 4 (enter nothing to exit):
4
Latency Constraint: 4
Generating schedule with integer linear programming.
Finding the mobility for each operation.
Setting up model for ILP.
Cycle      =  1,  2,  3,  4,  5
op1        =  1,  0,  0,  0,  0
op2        =  1,  0,  0,  0,  0
op3        =  0,  1,  0,  0,  0
op4        =  0,  1,  0,  0,  0
op5        =  0,  0,  1,  0,  0
op6        =  0,  0,  1,  0,  0
op7        =  0,  0,  0,  1,  0
Resources: MULT = 2, SUB = 1
Cycle 00: MULT -> {op1, op2}
Cycle 01: MULT -> {op3, op4}
Cycle 02: MULT -> {op5}, SUB -> {op6}
Cycle 03: SUB -> {op7}
You entered the compatibility array:
1 0 1 1 1
0 1 1 1 1
1 1 1 0 1
1 1 0 1 1
1 1 1 1 1
Clique Set:
Clique #0 (size = 3) = { 4  0  2 }
Clique #1 (size = 2) = { 3  1 }
You entered the compatibility array:
1 1
1 1
Clique Set:
Clique #0 (size = 2) = { 1  0 }
MULT00 -> op1, op3, op5
MULT01 -> op2, op4
SUB00 -> op6, op7
a          -> (00, 00) *___
b          -> (00, 00) *___
c          -> (00, 00) *___
d          -> (00, 00) *___
e          -> (00, 01) **__
f          -> (00, 01) **__
g          -> (00, 02) ***_
h          -> (00, 02) ***_
i          -> (03, 03) ___*
t0         -> (00, 01) **__
t1         -> (00, 01) **__
t2         -> (01, 02) _**_
t3         -> (01, 02) _**_
t4         -> (02, 03) __**
t5         -> (02, 03) __**
You entered the compatibility array:
1 0 0 0 0 0 0 0 1 0 0 1 1 1 1
0 1 0 0 0 0 0 0 1 0 0 1 1 1 1
0 0 1 0 0 0 0 0 1 0 0 1 1 1 1
0 0 0 1 0 0 0 0 1 0 0 1 1 1 1
0 0 0 0 1 0 0 0 1 0 0 0 0 1 1
0 0 0 0 0 1 0 0 1 0 0 0 0 1 1
0 0 0 0 0 0 1 0 1 0 0 0 0 0 0
0 0 0 0 0 0 0 1 1 0 0 0 0 0 0
1 1 1 1 1 1 1 1 1 1 1 1 1 0 0
0 0 0 0 0 0 0 0 1 1 0 0 0 1 1
0 0 0 0 0 0 0 0 1 0 1 0 0 1 1
1 1 1 1 0 0 0 0 1 0 0 1 0 0 0
1 1 1 1 0 0 0 0 1 0 0 0 1 0 0
1 1 1 1 1 1 0 0 0 1 1 0 0 1 0
1 1 1 1 1 1 0 0 0 1 1 0 0 0 1
Clique Set:
Clique #0 (size = 2) = { 8  9 }
Clique #1 (size = 2) = { 14  10 }
Clique #2 (size = 2) = { 13  4 }
Clique #3 (size = 2) = { 12  0 }
Clique #4 (size = 2) = { 11  1 }
Clique #5 (size = 1) = { 7 }
Clique #6 (size = 1) = { 6 }
Clique #7 (size = 1) = { 5 }
Clique #8 (size = 1) = { 3 }
Clique #9 (size = 1) = { 2 }
REG00 -> i, t0
REG01 -> t1, t5
REG02 -> e, t4
REG03 -> a, t3
REG04 -> b, t2
REG05 -> h
REG06 -> g
REG07 -> f
REG08 -> d
REG09 -> c
MX_MULT00 = {00: [a, b], 01: [e, f], 10: [g, t2]}
MX_MULT01 = {0: [c, d], 1: [t0, t1]}
MX_SUB00 = {0: [t3, h], 1: [t5, t4]}
MX_REG00 = {0: i, 1: t0}
MX_REG01 = {0: t1, 1: t5}
MX_REG02 = {0: e, 1: t4}
MX_REG03 = {0: a, 1: t3}
MX_REG04 = {0: b, 1: t2}
```
4. Next you can provide test cases to generate a test bench file.
```
i = f(a, b, c, d, e, f, g, h) = (a * b * c * d) - h - (g * e * f)
Do you want to add a test case (yes or no)?
yes
Enter the values for the inputs:
Enter the base 10 value for a:
2
Enter the base 10 value for b:
1
Enter the base 10 value for c:
2
Enter the base 10 value for d:
3
Enter the base 10 value for e:
2
Enter the base 10 value for f:
1
Enter the base 10 value for g:
2
Enter the base 10 value for h:
2
Enter the expected values for the outputs:
Enter the base 10 value for i:
6
Do you want to add a test case (yes or no)?
no
```
5. Provide a save path for all the files.
```
Provide a folder path to save the VHDL files. Enter nothing to exit.
docs\sample2
```
6. When the program finishes you will be prompted to press enter to exit the program.
```
Press enter to exit.
```

Somethings to keep in mind is that the synthesis tool only supports unsigned integers. Also keep in mind that overflow can happen between two variables which will affect the outcome.
