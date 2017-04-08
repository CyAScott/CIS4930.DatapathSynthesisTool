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

entity input is
	port(
	 a : IN std_logic_vector(3 downto 0);
	 b : IN std_logic_vector(3 downto 0);
	 c : IN std_logic_vector(3 downto 0);
	 d : IN std_logic_vector(3 downto 0);
	 e : IN std_logic_vector(3 downto 0);
	 f : IN std_logic_vector(3 downto 0);
	 g : IN std_logic_vector(3 downto 0);
	 h : IN std_logic_vector(3 downto 0);
	 i : OUT std_logic_vector(3 downto 0);
	 clock : IN Std_logic;
         s_tart : IN Std_logic;
         clear : IN Std_logic;
         Finish : OUT Std_logic);
end input;

architecture RTL of input is

  component input_controller
        port(clock, reset : IN Std_logic;
        s_tart : IN Std_logic;
        finish : Out Std_logic;
	control_out : Out Std_logic_vector(0 to 14));    
   end component;
   for all : input_controller use entity Work.input_controller(Moore);

  component input_dp
  port(	 a : IN std_logic_vector(3 downto 0);
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
	 clock: IN std_logic);
  end component;
  for all : input_dp use entity Work.input_dp(RTL);

  signal sig_con_out : Std_logic_Vector(0 to 14);

begin
  inputcon : input_controller
        port map(clock => clock,
                 s_tart => s_tart,
                 reset => clear,
		 finish => finish,
                 control_out => sig_con_out);

  inputdp : input_dp
        port map(
		a => a,
		b => b,
		c => c,
		d => d,
		e => e,
		f => f,
		g => g,
		h => h,
		i => i,
		ctrl(0) => sig_con_out(0),
		ctrl(1) => sig_con_out(1),
		ctrl(2) => sig_con_out(2),
		ctrl(3) => sig_con_out(3),
		ctrl(4) => sig_con_out(4),
		ctrl(5) => sig_con_out(5),
		ctrl(6) => sig_con_out(6),
		ctrl(7) => sig_con_out(7),
		ctrl(8) => sig_con_out(8),
		ctrl(9) => sig_con_out(9),
		ctrl(10) => sig_con_out(10),
		ctrl(11) => sig_con_out(11),
		ctrl(12) => sig_con_out(12),
		ctrl(13) => sig_con_out(13),
		ctrl(14) => sig_con_out(14),
		clock => clock,
		clear => clear);
end RTL;