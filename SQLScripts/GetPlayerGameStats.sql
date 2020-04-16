CREATE DEFINER=`root`@`localhost` PROCEDURE `GetPlayerGameStats`(PlayerID VARCHAR(255))
BEGIN

SELECT 
SUM(kills) AS "kills", 
SUM(deaths) as "deaths", 
SUM(score) as "score", 
COUNT(*) as "games" 
FROM played_match
WHERE played_match.player_id = PlayerID;

END