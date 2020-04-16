CREATE DEFINER=`root`@`localhost` PROCEDURE `GetPlayerItems`(PlayerID VARCHAR(255))
BEGIN

SELECT * FROM items WHERE owner_id = PlayerID;

END