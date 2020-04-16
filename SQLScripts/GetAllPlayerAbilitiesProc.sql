CREATE DEFINER=`root`@`localhost` PROCEDURE `Get_All_Player_Abilities`(PlayerID VARCHAR(255))
BEGIN

SELECT * FROM intrusive.abilities 
WHERE intrusive.abilities.ability_name IN 
(
	SELECT has_learned.ability_name FROM has_learned WHERE has_learned.player_id = PlayerID
);

END