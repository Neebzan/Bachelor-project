CREATE DEFINER=`root`@`localhost` PROCEDURE `GetPlayerEquippedItems`(PlayerID VARCHAR(255))
BEGIN

SELECT * FROM wears WHERE wears.player_id = PlayerID;

END