---------------------------------------------------------------------
--
-- Functional Unit Multiplexers:
--
--	MX_MULT00 = {00: [a, b], 01: [e, f], 10: [g, t2]}
--	MX_MULT01 = {0: [c, d], 1: [t0, t1]}
--	MX_SUB00 = {0: [t3, h], 1: [t5, t4]}
--
-- Register Multiplexers:
--
-- 	MX_REG00 = {0: i, 1: t0}
-- 	MX_REG01 = {0: t1, 1: t5}
-- 	MX_REG02 = {0: e, 1: t4}
-- 	MX_REG03 = {0: a, 1: t3}
-- 	MX_REG04 = {0: b, 1: t2}
--
-- Order of Operations:
--
-- Cycle 0: MULT.op1(a, b), MULT.op2(c, d)
-- Cycle 1: MULT.op4(t0, t1), MULT.op3(e, f)
-- Cycle 2: SUB.op6(t3, h), MULT.op5(g, t2)
-- Cycle 3: SUB.op7(t5, t4)
--
-- Expressions:
--
--	i = f(a, b, c, d, e, f, g, h) = (a * b * c * d) - h - (g * e * f)
--
---------------------------------------------------------------------

library IEEE;
use IEEE.std_logic_1164.all;

entity input_controller is
	port
	(
		clock, reset, s_tart : IN std_logic;
		finish : OUT std_logic;
		control_out : OUT std_logic_vector(0 to 18)
	);
end input_controller;

architecture moore of input_controller is

	signal current_state, next_state : integer := 0;
	signal internal_finish : std_logic := '0';
	signal control_bus : std_logic_vector(0 to 18) := "0000000000000000000";

begin

	process(current_state)
	begin
		case current_state is
			when 0 =>
				control_bus(0) <= '1'; -- REG00: store output from operation op1
				control_bus(1) <= '1'; -- REG01: store output from operation op2
				control_bus(2) <= '1'; -- REG02: store input e
				control_bus(3) <= '1'; -- REG03: store input a
				control_bus(4) <= '1'; -- REG04: store input b
				control_bus(5) <= '1'; -- REG05: store input h
				control_bus(6) <= '1'; -- REG06: store input g
				control_bus(7) <= '1'; -- REG07: store input f
				control_bus(8) <= '1'; -- REG08: store input d
				control_bus(9) <= '1'; -- REG09: store input c
				control_bus(10 to 11) <= "00"; -- 00 MX_MULT00: select op1
				control_bus(12) <= '0'; -- 0 MX_MULT01: select op2
				control_bus(13) <= 'X'; -- X MX_SUB00
				control_bus(14) <= '1'; -- 1 MX_REG00: select t0
				control_bus(15) <= '0'; -- 0 MX_REG01: select t1
				control_bus(16) <= '0'; -- 0 MX_REG02: select e
				control_bus(17) <= '0'; -- 0 MX_REG03: select a
				control_bus(18) <= '0'; -- 0 MX_REG04: select b
				-- Binary: 1111111111000X10000
				-- Hex: ffe10
				internal_finish <= '0';
				next_state <= 1;
			when 1 =>
				control_bus(0) <= '0'; -- REG00: keep value
				control_bus(1) <= '0'; -- REG01: keep value
				control_bus(2) <= '0'; -- REG02: keep value
				control_bus(3) <= '1'; -- REG03: store output from operation op4
				control_bus(4) <= '1'; -- REG04: store output from operation op3
				control_bus(5) <= '0'; -- REG05: keep value
				control_bus(6) <= '0'; -- REG06: keep value
				control_bus(7) <= '0'; -- REG07: keep value
				control_bus(8) <= '0'; -- REG08: keep value
				control_bus(9) <= '0'; -- REG09: keep value
				control_bus(10 to 11) <= "01"; -- 01 MX_MULT00: select op3
				control_bus(12) <= '1'; -- 1 MX_MULT01: select op4
				control_bus(13) <= 'X'; -- X MX_SUB00
				control_bus(14) <= '1'; -- 1 MX_REG00: select t0
				control_bus(15) <= '0'; -- 0 MX_REG01: select t1
				control_bus(16) <= '0'; -- 0 MX_REG02: select e
				control_bus(17) <= '1'; -- 1 MX_REG03: select t3
				control_bus(18) <= '1'; -- 1 MX_REG04: select t2
				-- Binary: 0001100000011X10011
				-- Hex: 1c0d3
				internal_finish <= '0';
				next_state <= 2;
			when 2 =>
				control_bus(0) <= '0'; -- REG00: keep value
				control_bus(1) <= '1'; -- REG01: store output from operation op6
				control_bus(2) <= '1'; -- REG02: store output from operation op5
				control_bus(3) <= '0'; -- REG03: keep value
				control_bus(4) <= '0'; -- REG04: keep value
				control_bus(5) <= '0'; -- REG05: keep value
				control_bus(6) <= '0'; -- REG06: keep value
				control_bus(7) <= '0'; -- REG07: keep value
				control_bus(8) <= '0'; -- REG08: keep value
				control_bus(9) <= '0'; -- REG09: keep value
				control_bus(10 to 11) <= "10"; -- 10 MX_MULT00: select op5
				control_bus(12) <= 'X'; -- X MX_MULT01
				control_bus(13) <= '0'; -- 0 MX_SUB00: select op6
				control_bus(14) <= 'X'; -- X MX_REG00
				control_bus(15) <= '1'; -- 1 MX_REG01: select t5
				control_bus(16) <= '1'; -- 1 MX_REG02: select t4
				control_bus(17) <= '1'; -- 1 MX_REG03: select t3
				control_bus(18) <= '1'; -- 1 MX_REG04: select t2
				-- Binary: 011000000010X0X1111
				-- Hex: 6010f
				internal_finish <= '0';
				next_state <= 3;
			when 3 =>
				control_bus(0) <= '1'; -- REG00: store output from operation op7
				control_bus(1) <= '0'; -- REG01: keep value
				control_bus(2) <= '0'; -- REG02: keep value
				control_bus(3) <= '0'; -- REG03: keep value
				control_bus(4) <= '0'; -- REG04: keep value
				control_bus(5) <= '0'; -- REG05: keep value
				control_bus(6) <= '0'; -- REG06: keep value
				control_bus(7) <= '0'; -- REG07: keep value
				control_bus(8) <= '0'; -- REG08: keep value
				control_bus(9) <= '0'; -- REG09: keep value
				control_bus(10 to 11) <= "XX"; -- XX MX_MULT00
				control_bus(12) <= 'X'; -- X MX_MULT01
				control_bus(13) <= '1'; -- 1 MX_SUB00: select op7
				control_bus(14) <= '0'; -- 0 MX_REG00: select i
				control_bus(15) <= '1'; -- 1 MX_REG01: select t5
				control_bus(16) <= '1'; -- 1 MX_REG02: select t4
				control_bus(17) <= 'X'; -- X MX_REG03
				control_bus(18) <= 'X'; -- X MX_REG04
				-- Binary: 1000000000XXX1011XX
				-- Hex: 8002c
				internal_finish <= '1';
				next_state <= 0;
			when others => null;
		end case;
	end process;

	process(clock, reset)
	begin
		if (reset = '1' and reset'event) then
			current_state <= 0;
			control_out <= control_bus;
			finish <= internal_finish;
		elsif (clock = '1' and clock'event) then
			current_state <= next_state;
			control_out <= control_bus;
			finish <= internal_finish;
		end if;
	end process;

end moore;
