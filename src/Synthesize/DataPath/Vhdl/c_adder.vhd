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
begin
	P0 : process (input1, input2)
		variable carry : std_logic := '0';
		variable overflow : std_logic := '0';
		variable temp : std_logic_vector(width - 1 downto 0);
	begin
		for i in 0 to width - 1 loop
			if input1(i) = '0' and input2(i) = '0' and carry = '0' then
				temp(i) := '0';
				carry := '0';
			elsif input1(i) = '0' and input2(i) = '0' and carry = '1' then
				temp(i) := '1';
				carry := '0';
			elsif input1(i) = '0' and input2(i) = '1' and carry = '0' then
				temp(i) := '1';
				carry := '0';
			elsif input1(i) = '0' and input2(i) = '1' and carry = '1' then
				temp(i) := '0';
				carry := '1';
			elsif input1(i) = '1' and input2(i) = '0' and carry = '0' then
				temp(i) := '1';
				carry := '0';
			elsif input1(i) = '1' and input2(i) = '0' and carry = '1' then
				temp(i) := '0';
				carry := '1';
			elsif input1(i) = '1' and input2(i) = '1' and carry = '0' then
				temp(i) := '0';
				carry := '1';
			elsif input1(i) = '1' and input2(i) = '1' and carry = '1' then
				temp(i) := '1';
				carry := '1';
			end if;
		end loop;
		
		output <= temp;
	end process;
end behavior;