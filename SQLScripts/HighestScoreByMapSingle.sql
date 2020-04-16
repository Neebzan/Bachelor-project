CREATE DEFINER=`root`@`localhost` PROCEDURE `HighestScoreByMapSingle`(MapName VARCHAR(255))
BEGIN

SELECT player_id, MAX(score) 
FROM played_match NATURAL JOIN matches 
WHERE map_name = MapName;

END