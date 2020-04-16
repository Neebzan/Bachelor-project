CREATE DEFINER=`root`@`localhost` PROCEDURE `GetPlayerOverallStats`(PlayerID VARCHAR(255))
BEGIN

SELECT stats.kills, stats.deaths, stats.score, stats.games, fav_map.map
FROM
(
SELECT 
SUM(kills) AS kills, 
SUM(deaths) as deaths, 
SUM(score) as score, 
COUNT(*) as games
FROM played_match WHERE played_match.player_id = PlayerID
) AS stats 
JOIN
(
SELECT map_name as map
FROM played_match 
INNER JOIN matches 
ON played_match.match_id = matches.match_id
WHERE player_id = PlayerID
GROUP BY map_name 
ORDER BY COUNT(*) DESC 
LIMIT 1
) AS fav_map;

END