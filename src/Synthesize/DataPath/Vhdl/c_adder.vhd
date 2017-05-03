library ieee;
use ieee.std_logic_1164.all;
library WORK;
use WORK.all;

entity c_adder is
	generic
	(
		width : integer := 4
	);
	port
	(
		input1, input2 : in std_logic_vector((width - 1) downto 0);
		output : out std_logic_vector((width - 1) downto 0)
	);
end c_adder;

architecture behavior of c_adder is
	function bits_to_int (input : std_logic_vector)return integer is
		variable ret_val : integer := 0;
	begin
		for i in input'range loop
			if input(i) = '1' then
				ret_val := 2 ** i + ret_val;
			end if;
		end loop;
		return ret_val;
	end bits_to_int;
begin
	process (input1, input2)
		variable value : integer;
		variable result : std_logic_vector((width - 1) downto 0);
	begin
		value := bits_to_int(input1) + bits_to_int(input2);

		for i in 0 to width - 1 loop
			if (value rem 2) = 1 then
				result(i) := '1';
			else
				result(i) := '0';
			end if;
			value := value / 2;
		end loop;
		
		output <= result;
	end process;
end behavior;