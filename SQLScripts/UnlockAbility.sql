CREATE DEFINER=`root`@`localhost` PROCEDURE `UnlockAbility`(PlayerID VARCHAR(255), AbilityName VARCHAR(255))
BEGIN

INSERT INTO has_learned (player_id, ability_name)
VALUES (PlayerID, AbilityName);

END