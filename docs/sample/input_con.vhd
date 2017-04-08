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

entity input_controller is
	port (clock, reset : IN Std_logic;
	s_tart : IN Std_logic;
	finish : OUT Std_logic;
	control_out : Out Std_logic_vector(0 to 14));
end input_controller;

architecture Moore of input_controller is

   type State_enum is ( S0,  S1,  S2,  S3,  S4,  S5,  S6 );

   signal Current_state : State_enum;
   signal Next_state : State_enum;
   signal Internal_finish : Std_logic;
   signal Control_bus : Std_logic_vector(0 to 14);

begin

  process(clock, Reset)
  begin
     if (Reset = '1') then
        Current_state <= S0;
     elsif (clock = '1' and clock'event) then
        Current_state <= Next_state;
     end if;
  end process;

  process
  begin
     wait until clock = '0';
     Control_out <= Control_bus;
     finish <= Internal_finish;
  end process;


  process(Current_state, s_tart)
  begin
     case Current_state is

       when S0 =>
            -- Timestep = 0
            Internal_finish <= '0';
            Control_bus <= "000000000000000";
             case s_tart is
		when '1' => Next_state <= S1;
		when '0' => Next_state <= S0;
		when others => null;
          end case;

       when S1 =>
            -- Timestep = 1
            Internal_finish <= '0';
            Control_bus <= "101111110110000";
            Next_state <= S2;

       when S2 =>
            -- Timestep = 2
            Internal_finish <= '0';
            Control_bus <= "010000001001000";
            Next_state <= S3;

       when S3 =>
            -- Timestep = 3
            Internal_finish <= '0';
            Control_bus <= "010000001000110";
            Next_state <= S4;

       when S4 =>
            -- Timestep = 4
            Internal_finish <= '0';
            Control_bus <= "010000001000001";
            Next_state <= S5;

       when S5 =>
            -- Timestep = 5
            Internal_finish <= '0';
            Control_bus <= "100000000000000";
            Next_state <= S6;

       when S6 =>
            -- Timestep = 6
            Internal_finish <= '1';
            Control_bus <= "000000000000000";
            Next_state <= S0;

     end case;
  end process;
end Moore;
