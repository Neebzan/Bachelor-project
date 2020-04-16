CREATE DEFINER=`root`@`localhost` PROCEDURE `GetPlayerMostPlayed_Map`(PlayerID VARCHAR(255))
BEGIN

SELECT map_name 
FROM played_match NATURAL JOIN matches 
GROUP BY map_name 
ORDER BY COUNT(*) DESC 
LIMIT 1;

END