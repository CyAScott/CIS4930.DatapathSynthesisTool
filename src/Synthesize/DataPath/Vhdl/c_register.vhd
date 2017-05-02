library IEEE;
use ieee.std_logic_1164.all;

entity c_register is
	generic
	(
		width : integer := 4
	);
	port
	(
		input : in std_logic_vector((width - 1) downto 0);
		wr : in std_logic;
		clear : in std_logic;
		clock : in std_logic;
		output : out std_logic_vector((width - 1) downto 0)
	);
end c_register;

architecture behavior of c_register is
begin
	process (clock, clear, input)
		variable interim_val : std_logic_vector((width - 1) downto 0);
	begin
		if (clear = '1') then
			for i in width - 1 downto 0 loop
				interim_val(i) := '0';
			end loop;
		elsif (wr = '1' and clock = '0' and (clock'event or input'event)) then
			interim_val := input;
		end if;

		output <= interim_val;
	end process;
end behavior;