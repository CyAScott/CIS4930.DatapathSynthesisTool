library ieee;
use ieee.std_logic_1164.all;
library WORK;
use WORK.all;

entity c_multiplexer is
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
end c_multiplexer;

architecture behavior of c_multiplexer is
begin
	P1 : process (mux_select, input)
		variable sel : integer;
	begin
		sel := 0;
		for i in select_size - 1 to 0 loop
			if mux_select(i) = '1' then
				sel := 2 ** i + sel;
			end if;
		end loop;
        output <= input(((sel + 1) * width - 1) downto (sel * width));
	end process P1;
end behavior;