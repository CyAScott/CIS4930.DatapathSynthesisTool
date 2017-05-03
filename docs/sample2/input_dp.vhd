---------------------------------------------------------------------
--
-- Registers:
--	REG00 (i, t0)
--	REG01 (t1, t5)
--	REG02 (e, t4)
--	REG03 (a, t3)
--	REG04 (b, t2)
--	REG05 (h)
--	REG06 (g)
--	REG07 (f)
--	REG08 (d)
--	REG09 (c)
-- Functional Units:
--	MULT00 (op1, op3, op5)
--	MULT01 (op2, op4)
--	SUB00 (op6, op7)
-- Multiplexers:
--	MX_MULT00 (op1, op3, op5)
--	MX_MULT01 (op2, op4)
--	MX_SUB00 (op6, op7)
--	MX_REG00 (i, t0)
--	MX_REG01 (t1, t5)
--	MX_REG02 (e, t4)
--	MX_REG03 (a, t3)
--	MX_REG04 (b, t2)
-- Expressions:
--	i = f(a, b, c, d, e, f, g, h) = (a * b * c * d) - h - (g * e * f)
--
---------------------------------------------------------------------

library IEEE;
use IEEE.std_logic_1164.all;

entity input_dp is
	port
	(
		a, b, c, d, e, f, g, h : IN std_logic_vector(3 downto 0);
		i : OUT std_logic_vector(3 downto 0);
		ctrl : IN std_logic_vector(0 to 18);
		clear, clock : IN std_logic
	);
end input_dp;

architecture rtl1 of input_dp is

	component c_multiplexer
		generic
		(
			width : integer := 4;
			no_of_inputs : integer := 2;
			select_size : integer := 1
		);
		port
		(
			input : in std_logic_vector(((width * no_of_inputs) - 1) downto 0);
			mux_select : in std_logic_vector ((select_size - 1) downto 0);
			output : out std_logic_vector ((width - 1) downto 0)
		);
	end component;
	for all : c_multiplexer use entity work.c_multiplexer(behavior);

	component c_register
		generic
		(
			width : integer := 4
		);
		port
		(
			input : in std_logic_vector((width - 1) downto 0);
			WR : in std_logic;
			clear : in std_logic;
			clock : in std_logic;
			output : out std_logic_vector((width - 1) downto 0)
		);
	end component;
	for all : c_register use entity work.c_register(behavior);

	component c_multiplier
		generic
		(
			width : integer := 4
		);
		port
		(
			input1 : std_logic_vector((width - 1) downto 0);
			input2 : std_logic_vector((width - 1) downto 0);
			output : out std_logic_vector((width - 1) downto 0)
		);
	end component;
	for all : c_multiplier use entity work.c_multiplier(behavior);

	component c_subtractor
		generic
		(
			width : integer := 4
		);
		port
		(
			input1, input2 : in std_logic_vector((width - 1) downto 0);
			output : out std_logic_vector((width - 1) downto 0)
		);
	end component;
	for all : c_subtractor use entity work.c_subtractor(behavior);

	-- Outputs of registers
	signal REG00_out, REG01_out, REG02_out, REG03_out, REG04_out, REG05_out, REG06_out, REG07_out, REG08_out, REG09_out : std_logic_vector(3 downto 0);

	-- Outputs of FUs
	signal MULT00_out, MULT01_out, SUB00_out : std_logic_vector(3 downto 0);

	-- Outputs of Interconnect Units
	signal MX_MULT00_out, MX_MULT01_out, MX_SUB00_out : std_logic_vector(7 downto 0);
	signal MX_REG00_out, MX_REG01_out, MX_REG02_out, MX_REG03_out, MX_REG04_out : std_logic_vector(3 downto 0);

begin

	-- Registers

	-- REG00 (i, t0)
	REG00 : c_register
		generic map(4)
		port map
		(
			input(3 downto 0) => MX_REG00_out(3 downto 0), -- Items: i, t0
			wr => ctrl(0),
			clear => clear,
			clock => clock,
			output => REG00_out
		);

	-- REG01 (t1, t5)
	REG01 : c_register
		generic map(4)
		port map
		(
			input(3 downto 0) => MX_REG01_out(3 downto 0), -- Items: t1, t5
			wr => ctrl(1),
			clear => clear,
			clock => clock,
			output => REG01_out
		);

	-- REG02 (e, t4)
	REG02 : c_register
		generic map(4)
		port map
		(
			input(3 downto 0) => MX_REG02_out(3 downto 0), -- Items: e, t4
			wr => ctrl(2),
			clear => clear,
			clock => clock,
			output => REG02_out
		);

	-- REG03 (a, t3)
	REG03 : c_register
		generic map(4)
		port map
		(
			input(3 downto 0) => MX_REG03_out(3 downto 0), -- Items: a, t3
			wr => ctrl(3),
			clear => clear,
			clock => clock,
			output => REG03_out
		);

	-- REG04 (b, t2)
	REG04 : c_register
		generic map(4)
		port map
		(
			input(3 downto 0) => MX_REG04_out(3 downto 0), -- Items: b, t2
			wr => ctrl(4),
			clear => clear,
			clock => clock,
			output => REG04_out
		);

	-- REG05 (h)
	REG05 : c_register
		generic map(4)
		port map
		(
			input(3 downto 0) => h(3 downto 0),
			wr => ctrl(5),
			clear => clear,
			clock => clock,
			output => REG05_out
		);

	-- REG06 (g)
	REG06 : c_register
		generic map(4)
		port map
		(
			input(3 downto 0) => g(3 downto 0),
			wr => ctrl(6),
			clear => clear,
			clock => clock,
			output => REG06_out
		);

	-- REG07 (f)
	REG07 : c_register
		generic map(4)
		port map
		(
			input(3 downto 0) => f(3 downto 0),
			wr => ctrl(7),
			clear => clear,
			clock => clock,
			output => REG07_out
		);

	-- REG08 (d)
	REG08 : c_register
		generic map(4)
		port map
		(
			input(3 downto 0) => d(3 downto 0),
			wr => ctrl(8),
			clear => clear,
			clock => clock,
			output => REG08_out
		);

	-- REG09 (c)
	REG09 : c_register
		generic map(4)
		port map
		(
			input(3 downto 0) => c(3 downto 0),
			wr => ctrl(9),
			clear => clear,
			clock => clock,
			output => REG09_out
		);

	-- Functional Units

	-- MULT00 MULT(op1, op3, op5)
	MULT00 : c_multiplier
		generic map(4)
		port map
		(
			input1(3 downto 0) => MX_MULT00_out(3 downto 0), -- a, e, g
			input2(3 downto 0) => MX_MULT00_out(7 downto 4), -- b, f, t2
			output(3 downto 0) => MULT00_out(3 downto 0)
		);

	-- MULT01 MULT(op2, op4)
	MULT01 : c_multiplier
		generic map(4)
		port map
		(
			input1(3 downto 0) => MX_MULT01_out(3 downto 0), -- c, t0
			input2(3 downto 0) => MX_MULT01_out(7 downto 4), -- d, t1
			output(3 downto 0) => MULT01_out(3 downto 0)
		);

	-- SUB00 SUB(op6, op7)
	SUB00 : c_subtractor
		generic map(4)
		port map
		(
			input1(3 downto 0) => MX_SUB00_out(3 downto 0), -- t3, t5
			input2(3 downto 0) => MX_SUB00_out(7 downto 4), -- h, t4
			output(3 downto 0) => SUB00_out(3 downto 0)
		);

	-- Multiplexers

	-- MX_MULT00: op1, op3, op5
	MX_MULT00 : c_multiplexer
		generic map(8, 3, 2)
		port map
		(
			-- Operation op1: MULT(a, b)
			input(3 downto 0) => REG03_out(3 downto 0), -- a
			input(7 downto 4) => REG04_out(3 downto 0), -- b
			-- Operation op3: MULT(e, f)
			input(11 downto 8) => REG02_out(3 downto 0), -- e
			input(15 downto 12) => REG07_out(3 downto 0), -- f
			-- Operation op5: MULT(g, t2)
			input(19 downto 16) => REG06_out(3 downto 0), -- g
			input(23 downto 20) => REG04_out(3 downto 0), -- t2
			mux_select(1 downto 0) => ctrl(10 to 11),
			output => MX_MULT00_out
		);

	-- MX_MULT01: op2, op4
	MX_MULT01 : c_multiplexer
		generic map(8, 2, 1)
		port map
		(
			-- Operation op2: MULT(c, d)
			input(3 downto 0) => REG09_out(3 downto 0), -- c
			input(7 downto 4) => REG08_out(3 downto 0), -- d
			-- Operation op4: MULT(t0, t1)
			input(11 downto 8) => REG00_out(3 downto 0), -- t0
			input(15 downto 12) => REG01_out(3 downto 0), -- t1
			mux_select(0) => ctrl(12),
			output => MX_MULT01_out
		);

	-- MX_SUB00: op6, op7
	MX_SUB00 : c_multiplexer
		generic map(8, 2, 1)
		port map
		(
			-- Operation op6: SUB(t3, h)
			input(3 downto 0) => REG03_out(3 downto 0), -- t3
			input(7 downto 4) => REG05_out(3 downto 0), -- h
			-- Operation op7: SUB(t5, t4)
			input(11 downto 8) => REG01_out(3 downto 0), -- t5
			input(15 downto 12) => REG02_out(3 downto 0), -- t4
			mux_select(0) => ctrl(13),
			output => MX_SUB00_out
		);

	-- MX_REG00: i, t0
	MX_REG00 : c_multiplexer
		generic map(4, 2, 1)
		port map
		(
			input(3 downto 0) => SUB00_out(3 downto 0), -- i
			input(7 downto 4) => MULT00_out(3 downto 0), -- t0
			mux_select(0) => ctrl(14),
			output => MX_REG00_out
		);

	-- MX_REG01: t1, t5
	MX_REG01 : c_multiplexer
		generic map(4, 2, 1)
		port map
		(
			input(3 downto 0) => MULT01_out(3 downto 0), -- t1
			input(7 downto 4) => SUB00_out(3 downto 0), -- t5
			mux_select(0) => ctrl(15),
			output => MX_REG01_out
		);

	-- MX_REG02: e, t4
	MX_REG02 : c_multiplexer
		generic map(4, 2, 1)
		port map
		(
			input(3 downto 0) => e(3 downto 0), -- e
			input(7 downto 4) => MULT00_out(3 downto 0), -- t4
			mux_select(0) => ctrl(16),
			output => MX_REG02_out
		);

	-- MX_REG03: a, t3
	MX_REG03 : c_multiplexer
		generic map(4, 2, 1)
		port map
		(
			input(3 downto 0) => a(3 downto 0), -- a
			input(7 downto 4) => MULT01_out(3 downto 0), -- t3
			mux_select(0) => ctrl(17),
			output => MX_REG03_out
		);

	-- MX_REG04: b, t2
	MX_REG04 : c_multiplexer
		generic map(4, 2, 1)
		port map
		(
			input(3 downto 0) => b(3 downto 0), -- b
			input(7 downto 4) => MULT00_out(3 downto 0), -- t2
			mux_select(0) => ctrl(18),
			output => MX_REG04_out
		);

	-- Primary outputs
	i(3 downto 0) <= REG00_out(3 downto 0);

end rtl1;
