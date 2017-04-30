library ieee;
use ieee.std_logic_1164.all;
library WORK;
use WORK.all;

entity c_concat is
	generic
	(
		width1, width2 : integer := 4
	);
	port
	(
		input1 : in std_logic_vector((width1 - 1) downto 0);
		input2 : in std_logic_vector((width2 - 1) downto 0);
		output : out std_logic_vector((width1 + width2 - 1) downto 0)
	);
end c_concat;

architecture behavior of c_concat is
begin
	P0 : process (input1, input2)
	begin
		for1 : for i in 0 to (width2 - 1) loop
			output(i) <= input2(i);
		end loop for1;
		for2 : for i in 0 to (width1 - 1) loop
			output(width2 + i) <= input1(i);
		end loop for2;
	end process P0;
end behavior;