library ieee;
use ieee.std_logic_1164.all;
library WORK;
use WORK.all;

entity c_subtractor is
	generic
	(
		width : integer := 4
	);
	port
	(
		input1, input2 : in std_logic_vector((width - 1) downto 0);
		output : out std_logic_vector(width downto 0)
	);
end c_subtractor;

architecture behavior of c_subtractor is
	function bits_to_int (input : std_logic_vector) return integer is
		variable ret_val : integer := 0;
	begin
		for i in input'range loop
			if input(i) = '1' then
				ret_val := 2 ** i + ret_val;
			end if;
		end loop; return ret_val;
	end bits_to_int;
begin
	P0 : process (input1, input2)
		variable difference : integer := 0;
		variable temp : std_logic_vector(width downto 0);
	begin
		difference := bits_to_int(input1) - bits_to_int(input2);

		if (difference < 0) then
			difference := (2 ** (width + 1)) + difference;
		end if;
		
		for i in 0 to width loop
			if (difference rem 2) = 1 then
				temp(i) := '1';
			else
				temp(i) := '0';
			end if;
			difference := difference/2;
		end loop;
		
		output <= temp;
	end process;
end behavior;