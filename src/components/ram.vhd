library ieee;
use ieee.std_logic_1164.all;
library WORK;
use WORK.all;

entity ram is
	generic
	(
		width : integer;
		ram_select : integer
	);
	port
	(
		input1 : in std_logic_vector((width - 1) downto 0);
		input2 : in std_logic_vector((ram_select - 1) downto 0);
		wr, rd, clock : in std_logic;
		output : out std_logic_vector((width - 1) downto 0)
	);
end ram;

architecture behavior of ram is
	function bits_to_int (input : std_logic_vector)return integer is
		variable ret_val : integer := 0;
	begin
		for i in input'range loop
			if input(i) = '1' then
				ret_val := 2 ** i + ret_val;
			end if;
		end loop; return ret_val;
	end bits_to_int;

	type mem_type is array(0 to (2 ** ram_select - 1)) of
	std_logic_vector((width - 1) downto 0);

	signal mem_storage : mem_type;
begin
	P0 : process (rd, wr, input1, input2, clock)
	begin
		if (clock = '1' and clock'EVENT) then
			if (rd = '1') then
				output <= mem_storage(bits_to_int(input2));
			end if;
			if (wr = '1') then
				mem_storage(bits_to_int(input2)) <= input1;
			end if;
		end if;
	end process P0;
end behavior;