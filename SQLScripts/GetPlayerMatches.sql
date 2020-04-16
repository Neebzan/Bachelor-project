CREATE DEFINER=`root`@`localhost` PROCEDURE `GetPlayerMatches`(PlayerID VARCHAR(255))
BEGIN

SELECT * FROM matches 
WHERE matches.match_id IN 
(
	SELECT played_match.match_id FROM played_match WHERE played_match.player_id = PlayerID
);

END