CREATE DEFINER=`root`@`localhost` PROCEDURE `HighscoreExp`(IN PlayerID VARCHAR(255))
BEGIN

SET @exp = (SELECT experience FROM players WHERE player_id = PlayerID);
SET @player_pos = (SELECT COUNT(*) FROM players WHERE experience >= @exp);
SET @asc_count = @player_pos+1;
SET @desc_count = @player_pos;

SELECT * FROM
( (
SELECT *,(@asc_count:=@asc_count -1) as pos
FROM players
WHERE experience >= @exp ORDER BY experience ASC LIMIT 11
) 
UNION ALL
(
SELECT *,(@desc_count:=@desc_count +1) as pos
FROM players
WHERE experience < @exp ORDER BY experience DESC LIMIT 10
)) as ExpHighscore ORDER BY experience DESC;

END