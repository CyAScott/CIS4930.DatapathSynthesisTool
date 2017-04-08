---------------------------------------------------------------------  
-- 								   -- 	
--  This file is generated automatically by AUDI (AUtomatic 	   -- 	
--  Design Instantiation) system, a behavioral synthesis system,   -- 	
--  developed at the University of South Florida.  This project    -- 	
--  is supported by the National Science Foundation (NSF) under    -- 	
--  the project number XYZ.  If you have any questions, contact    -- 	
--  Dr. Srinivas Katkoori (katkoori@csee.usf.edu), Computer        -- 	
--  Science & Engineering Department, University of South Florida, -- 	 
--  Tampa, FL 33647.						   -- 	
--								   --	
--------------------------------------------------------------------- 	
--
--  Date & Time:
--  User login id/name: 
-- 
--  File name: 
--  Type: 
-- 
--  Input aif file name: 
-- 
--  CDFG statistics: 
--    * Number of PI's: 
--    * Number of PO's: 
--    * Number of internal edges: 
--    * Number of Operations: 
--    * Conditionals: 
--    * Loops: 
--    * Types of Operations: 
-- 
--  Design Flow/Algorithm Information:
--    * Scheduling Algorithm: 
--    * Allocation: 
--    * Binding: 
--    Interconnect style: Multiplexor-Based or Bus-based
-- 
--  Design Information: 
-- 
--    Datapath:
--     * Registers:
--     * Functional units:
--     * Number of Multiplexors: 
--     * Number of Buses:
--
--     * Operator Binding Information:
--
--     * Register Optimization Information:
--
--     * Register Binding Information:
--
-- 
--    Controller:
--     * Type: Moore/Mealy
--     * Number of states:  
--     * Number of control bits: 
--    
---------------------------------------------------------------------

library IEEE;
use IEEE.std_logic_1164.all;

library Beh_Lib;
use Beh_Lib.all;

entity input_dp is
 port( 	 a : IN std_logic_vector(3 downto 0);
	 b : IN std_logic_vector(3 downto 0);
	 c : IN std_logic_vector(3 downto 0);
	 d : IN std_logic_vector(3 downto 0);
	 e : IN std_logic_vector(3 downto 0);
	 f : IN std_logic_vector(3 downto 0);
	 g : IN std_logic_vector(3 downto 0);
	 h : IN std_logic_vector(3 downto 0);
	 i : OUT std_logic_vector(3 downto 0);
	 ctrl: IN std_logic_vector(14 downto 0);
	 clear: IN std_logic;
	 clock: IN std_logic
);
end input_dp;

architecture RTL of input_dp is

  component c_register
  generic (width : integer := 4);
  port (input : in std_logic_vector((width-1) downto 0);
        WR: in std_logic;
        clear : in std_logic;
        clock : in std_logic;
        output : out std_logic_vector((width -1) downto 0));
  end component;
  -- for all : c_register use entity Beh_Lib.c_register(behavior);

   component C_Latch
	generic (width : integer);
	port(	input : in Std_logic_vector ((width - 1) downto 0);
	ENABLE : in Std_logic; 
	CLEAR : in Std_logic; 
	CLOCK : in Std_logic; 
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : C_Latch use entity Beh_Lib.C_Latch(Behavior); 
 
   component Constant_Reg
	generic (width : integer;
		const : integer); 
	port(	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : Constant_Reg use entity Beh_Lib.Constant_Reg(Behavior); 
 
   component Shift_Reg
	generic (width : integer); 
	port(	input : in Std_logic_vector ((width - 1) downto 0);
	CONTROL : in Std_logic_vector (1 downto 0);
	CLEAR : in Std_logic; 
	CLOCK : in Std_logic; 
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : Shift_Reg use entity Beh_Lib.Shift_Reg(Behavior); 
 
   component C_Signal
	generic (width : integer); 
	port(	input : in Std_logic_vector ((width - 1) downto 0);
	STORE : in Std_logic; 
	UPDATE : in Std_logic; 
	CLEAR : in Std_logic; 
	CLOCK :  in Std_logic; 
	output : out Std_logic_vector ((width + 1) downto 0)); 
   end component; 
   -- for all : C_Signal use entity Beh_Lib.C_Signal(Behavior); 
 
   component Ram
	generic (width : integer;
		ram_select : integer); 
	port(	input1 : in Std_logic_vector ((width - 1) downto 0); 
	input2 : in Std_logic_vector ((ram_select - 1) downto 0); 
	WR : in Std_logic; 
	RD : in Std_logic; 
	CLOCK : in Std_logic; 
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : Ram use entity Beh_Lib.Ram(Behavior); 
 
   component C_Adder
	generic (width : integer); 
	port(	input1 : in Std_logic_vector ((width - 1) downto 0); 
	input2 : in Std_logic_vector ((width - 1) downto 0); 
	output : out Std_logic_vector (width downto 0)); 
   end component; 
   -- for all : C_Adder use entity Beh_Lib.C_Adder(Behavior); 
 
   component C_subtractor
	generic (width : integer); 
	port(	input1 : in Std_logic_vector ((width - 1) downto 0); 
	input2 : in Std_logic_vector ((width - 1) downto 0); 
	output : out Std_logic_vector (width downto 0)); 
   end component; 
   -- for all : C_Subtractor use entity Beh_Lib.C_Subtractor(Behavior); 
 
   component C_Comparator
	generic (width : integer); 
	port(	input1 : in Std_logic_vector ((width - 1) downto 0); 
	input2 : in Std_logic_vector ((width - 1) downto 0); 
	output : out Std_logic_vector (2 downto 0)); 
   end component; 
   -- for all : C_Comparator use entity Beh_Lib.C_Comparator(Behavior); 
 
   component C_multiplier
	generic (width : integer); 
	port(	input1 : in Std_logic_vector ((width - 1) downto 0); 
	input2 : in Std_logic_vector ((width - 1) downto 0); 
	output : out Std_logic_vector (((width * 2) - 2) downto 0)); 
   end component; 
   -- for all : C_Multiplier use entity Beh_Lib.C_Multiplier(Behavior); 
 
   component C_Divider
	generic (width : integer;
		const : integer); 
	port(	input : in Std_logic_vector ((width - 1) downto 0);
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : C_Divider use entity Beh_Lib.C_Divider(Behavior); 
 
   component C_Concat
	generic (width1: integer;
		width2 : integer);
	port(	input1 : in Std_logic_vector ((width1 - 1) downto 0); 
	input2 : in Std_logic_vector ((width2 - 1) downto 0); 
	output : out Std_logic_vector (((width1 + width2) - 1) downto 0)); 
   end component; 
   -- for all : C_Concat use entity Beh_Lib.C_Concat(Behavior); 
 
   component C_Multiplexer
	generic (width : integer;
		no_of_inputs : integer;
		select_size : integer); 
	port(	input : in Std_logic_vector (((width*no_of_inputs) - 1) downto 0);
	MUX_SELECT : in Std_logic_vector ((select_size - 1) downto 0);
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : C_Multiplexer use entity Beh_Lib.C_Multiplexer(Behavior); 
 
   component C_Bus
	generic (width : integer;
		no_of_inputs : integer); 
	port(	input : in Std_logic_vector (((width*no_of_inputs) - 1) downto 0);
	BUS_SELECT : in Std_logic_vector ((no_of_inputs - 1) downto 0);
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : C_Bus use entity Beh_Lib.C_Bus(Behavior); 
 
   component C_And
	generic (width : integer); 
	port(	input1 : in Std_logic_vector ((width - 1) downto 0); 
	input2 : in Std_logic_vector ((width - 1) downto 0); 
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : C_And use entity Beh_Lib.C_And(Behavior); 
 
   component C_Or
	generic (width : integer); 
	port(	input1 : in Std_logic_vector ((width - 1) downto 0); 
	input2 : in Std_logic_vector ((width - 1) downto 0); 
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : C_Or use entity Beh_Lib.C_Or(Behavior); 
 
   component C_Nand
	generic (width : integer); 
	port(	input1 : in Std_logic_vector ((width - 1) downto 0); 
	input2 : in Std_logic_vector ((width - 1) downto 0); 
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : C_Nand use entity Beh_Lib.C_Nand(Behavior); 
 
   component C_Nor
	generic (width : integer); 
	port(	input1 : in Std_logic_vector ((width - 1) downto 0); 
	input2 : in Std_logic_vector ((width - 1) downto 0); 
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : C_Nor use entity Beh_Lib.C_Nor(Behavior); 
 
   component C_XNor
	generic (width : integer); 
	port(	input1 : in Std_logic_vector ((width - 1) downto 0); 
	input2 : in Std_logic_vector ((width - 1) downto 0); 
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : C_XNor use entity Beh_Lib.C_XNor(Behavior); 
 
   component C_Xor
	generic (width : integer); 
	port(	input1 : in Std_logic_vector ((width - 1) downto 0); 
	input2 : in Std_logic_vector ((width - 1) downto 0); 
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : C_Xor use entity Beh_Lib.C_Xor(Behavior); 
 
   component C_Not
	generic (width : integer); 
	port(	input : in Std_logic_vector ((width - 1) downto 0);
	output : out Std_logic_vector ((width - 1) downto 0)); 
   end component; 
   -- for all : C_Not use entity Beh_Lib.C_Not(Behavior); 
 
   component Tri_State_Buf
	generic (width : integer);
	port (input : in Std_logic_vector ((width - 1) downto 0);
	enable : in Std_logic;
	output : out Std_logic_vector ((width - 1) downto 0));
   end component; 
   -- for all : Tri_State_Buf use entity Beh_Lib.Tri_State_Buf(Behavior);

	-- Outputs of registers 
	 signal R0_out : Std_logic_vector(3 downto 0);
	 signal R1_out : Std_logic_vector(3 downto 0);
	 signal R2_out : Std_logic_vector(3 downto 0);
	 signal R3_out : Std_logic_vector(3 downto 0);
	 signal R4_out : Std_logic_vector(3 downto 0);
	 signal R5_out : Std_logic_vector(3 downto 0);
	 signal R6_out : Std_logic_vector(3 downto 0);
	 signal R7_out : Std_logic_vector(3 downto 0);
	 signal R8_out : Std_logic_vector(3 downto 0);
	 signal R9_out : Std_logic_vector(3 downto 0);

	-- Outputs of FUs 
	 signal FU0_0_out : Std_logic_vector(4 downto 0);
	 signal FU0_1_out : Std_logic_vector(4 downto 0);
	 signal FU0_2_out : Std_logic_vector(4 downto 0);
	 signal FU0_3_out : Std_logic_vector(4 downto 0);
	 signal FU0_4_out : Std_logic_vector(4 downto 0);
	 signal FU1_0_out : Std_logic_vector(4 downto 0);
	 signal FU1_1_out : Std_logic_vector(4 downto 0);

	-- Outputs of Interconnect Units 
	 signal Mux0_out :  Std_logic_vector(3 downto 0);
	 signal Mux1_out :  Std_logic_vector(3 downto 0);
	 signal Mux2_out :  Std_logic_vector(3 downto 0);
begin

	R0 : C_Register
	 generic map(4)
	 port map ( 
		 input(3 downto 0) => Mux0_out(3 downto 0),
		 WR => ctrl(0),
		 CLEAR => clear,
		 CLOCK => clock,
		 output => R0_out);

	R1 : C_Register
	 generic map(4)
	 port map ( 
		 input(3 downto 0) => Mux1_out(3 downto 0),
		 WR => ctrl(1),
		 CLEAR => clear,
		 CLOCK => clock,
		 output => R1_out);

	R2 : C_Register
	 generic map(4)
	 port map ( 
		 input(3 downto 0) => h(3 downto 0),
		 WR => ctrl(2),
		 CLEAR => clear,
		 CLOCK => clock,
		 output => R2_out);

	R3 : C_Register
	 generic map(4)
	 port map ( 
		 input(3 downto 0) => g(3 downto 0),
		 WR => ctrl(3),
		 CLEAR => clear,
		 CLOCK => clock,
		 output => R3_out);

	R4 : C_Register
	 generic map(4)
	 port map ( 
		 input(3 downto 0) => f(3 downto 0),
		 WR => ctrl(4),
		 CLEAR => clear,
		 CLOCK => clock,
		 output => R4_out);

	R5 : C_Register
	 generic map(4)
	 port map ( 
		 input(3 downto 0) => e(3 downto 0),
		 WR => ctrl(5),
		 CLEAR => clear,
		 CLOCK => clock,
		 output => R5_out);

	R6 : C_Register
	 generic map(4)
	 port map ( 
		 input(3 downto 0) => d(3 downto 0),
		 WR => ctrl(6),
		 CLEAR => clear,
		 CLOCK => clock,
		 output => R6_out);

	R7 : C_Register
	 generic map(4)
	 port map ( 
		 input(3 downto 0) => c(3 downto 0),
		 WR => ctrl(7),
		 CLEAR => clear,
		 CLOCK => clock,
		 output => R7_out);

	R8 : C_Register
	 generic map(4)
	 port map ( 
		 input(3 downto 0) => Mux2_out(3 downto 0),
		 WR => ctrl(8),
		 CLEAR => clear,
		 CLOCK => clock,
		 output => R8_out);

	R9 : C_Register
	 generic map(4)
	 port map ( 
		 input(3 downto 0) => b(3 downto 0),
		 WR => ctrl(9),
		 CLEAR => clear,
		 CLOCK => clock,
		 output => R9_out);

	MULT0_0 : C_Multiplier 
		 generic map(4)
		 port map (
		 input1(3 downto 0) => R0_out(3 downto 0),
		 input2(3 downto 0) => R9_out(3 downto 0),
		 output(4 downto 0) => FU0_0_out(4 downto 0));


	MULT0_1 : C_Multiplier 
		 generic map(4)
		 port map (
		 input1(3 downto 0) => R7_out(3 downto 0),
		 input2(3 downto 0) => R6_out(3 downto 0),
		 output(4 downto 0) => FU0_1_out(4 downto 0));


	MULT0_2 : C_Multiplier 
		 generic map(4)
		 port map (
		 input1(3 downto 0) => R5_out(3 downto 0),
		 input2(3 downto 0) => R4_out(3 downto 0),
		 output(4 downto 0) => FU0_2_out(4 downto 0));


	MULT0_3 : C_Multiplier 
		 generic map(4)
		 port map (
		 input1(3 downto 0) => R8_out(3 downto 0),
		 input2(3 downto 0) => R1_out(3 downto 0),
		 output(4 downto 0) => FU0_3_out(4 downto 0));


	MULT0_4 : C_Multiplier 
		 generic map(4)
		 port map (
		 input1(3 downto 0) => R3_out(3 downto 0),
		 input2(3 downto 0) => R8_out(3 downto 0),
		 output(4 downto 0) => FU0_4_out(4 downto 0));


	SUB1_0 : C_Subtractor 
		 generic map(4)
		 port map (
		 input1(3 downto 0) => R1_out(3 downto 0),
		 input2(3 downto 0) => R2_out(3 downto 0),
		 output(4 downto 0) => FU1_0_out(4 downto 0));


	SUB1_1 : C_Subtractor 
		 generic map(4)
		 port map (
		 input1(3 downto 0) => R1_out(3 downto 0),
		 input2(3 downto 0) => R8_out(3 downto 0),
		 output(4 downto 0) => FU1_1_out(4 downto 0));


	Mux0 : C_Multiplexer
		generic map(4, 2, 1)
		port map(
		input(3 downto 0) => FU1_1_out(3 downto 0),
		input(7 downto 4) => a(3 downto 0),
		MUX_SELECT(0 downto 0) => ctrl(10 downto 10),
		output => Mux0_out);

	Mux1 : C_Multiplexer
		generic map(4, 3, 2)
		port map(
		input(3 downto 0) => FU1_0_out(3 downto 0),
		input(7 downto 4) => FU0_1_out(3 downto 0),
		input(11 downto 8) => FU0_3_out(3 downto 0),
		MUX_SELECT(1 downto 0) => ctrl(12 downto 11),
		output => Mux1_out);

	Mux2 : C_Multiplexer
		generic map(4, 3, 2)
		port map(
		input(3 downto 0) => FU0_0_out(3 downto 0),
		input(7 downto 4) => FU0_2_out(3 downto 0),
		input(11 downto 8) => FU0_4_out(3 downto 0),
		MUX_SELECT(1 downto 0) => ctrl(14 downto 13),
		output => Mux2_out);

	 -- Primary outputs 
	 i(3 downto 0) <= R0_out(3 downto 0);
end RTL;
