---------------------------------------------------------------------
--
-- Inputs: a, b, c, d, e, f, g, h
-- Output(s): i
-- Expressions:
--	i = f(a, b, c, d, e, f, g, h) = (a * b * c * d) - h - (g * e * f)
--
---------------------------------------------------------------------

library IEEE;
use IEEE.std_logic_1164.all;

entity input is
	port
	(
		a, b, c, d, e, f, g, h : IN std_logic_vector(3 downto 0);
		i : OUT std_logic_vector(3 downto 0);
		clear, clock, s_tart : IN std_logic;
		finish : OUT std_logic
	);
end input;

architecture rtl2 of input is

	component input_controller
		port
		(
			clock, reset, s_tart : IN std_logic;
			finish : OUT std_logic;
			control_out : OUT std_logic_vector(0 to 18)
		);
	end component;
	for all : input_controller use entity work.input_controller(moore);

	component input_dp
		port
		(
			a, b, c, d, e, f, g, h : IN std_logic_vector(3 downto 0);
			i : OUT std_logic_vector(3 downto 0);
			ctrl : IN std_logic_vector(0 to 18);
			clear, clock : IN std_logic
		);
	end component;
	for all : input_dp use entity work.input_dp(rtl1);

	signal sig_con_out : std_logic_vector(0 to 18);

begin

	inputcon : input_controller
		port map
		(
			clock => clock,
			s_tart => s_tart,
			reset => clear,
			finish => finish,
			control_out => sig_con_out
		);

	inputdp : input_dp
		port map
		(
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
			ctrl(15) => sig_con_out(15),
			ctrl(16) => sig_con_out(16),
			ctrl(17) => sig_con_out(17),
			ctrl(18) => sig_con_out(18),
			clock => clock,
			clear => clear
		);
end rtl2;
