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

entity test_me_tb is
end test_me_tb;

architecture test of test_me_tb is

	component input
		port
		(
			a, b, c, d, e, f, g, h : IN std_logic_vector(3 downto 0);
			i : OUT std_logic_vector(3 downto 0);
			clear, clock, s_tart : IN std_logic;
			finish : OUT std_logic
		);
	end component;
	for all : input use entity work.input(rtl);

	signal a, b, c, d, e, f, g, h, i : std_logic_vector(3 downto 0) := "0000";
	signal clear, clock, finish, s_tart : std_logic := '0';

begin

	test_input : input port map (a, b, c, d, e, f, g, h, i, clear, clock, s_tart, finish);

	process
		begin
			wait for 1 ns;

			-- i = f(a, b, c, d, e, f, g, h) = (a * b * c * d) - h - (g * e * f)
			a <= "0010"; -- 2
			b <= "0001"; -- 1
			c <= "0010"; -- 2
			d <= "0011"; -- 3
			e <= "0010"; -- 2
			f <= "0001"; -- 1
			g <= "0010"; -- 2
			h <= "0010"; -- 2
			s_tart <= '1';
			clock <= '1'; wait for 1 ns;
			clock <= '0'; wait for 1 ns;
			clock <= '1'; wait for 1 ns;
			clock <= '0'; wait for 1 ns;
			clock <= '1'; wait for 1 ns;
			clock <= '0'; wait for 1 ns;
			clock <= '1'; wait for 1 ns;
			clock <= '0'; wait for 1 ns;
			assert i = "0110" report "6 = f(2, 1, 2, 3, 2, 1, 2, 2) = (2 * 1 * 2 * 3) - 2 - (2 * 2 * 1)" severity failure;

			wait;
	end process;
end test;
