CREATE DATABASE intrusive;
USE intrusive;

-- Enitites
CREATE TABLE accounts (
	account_id VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    first_name VARCHAR(255),
    last_name VARCHAR(255),
    password_hash VARCHAR(255) NOT NULL,
    PRIMARY KEY (account_id)
);

CREATE TABLE players (
	player_id VARCHAR(255) NOT NULL,
    experience INT ,
    PRIMARY KEY (player_id),
    FOREIGN KEY (player_id) REFERENCES accounts(account_id)
);

CREATE TABLE map (
    map_name VARCHAR(255) NOT NULL,
    PRIMARY KEY (map_name)
);

CREATE TABLE matches (
	match_id INT NOT NULL AUTO_INCREMENT,
    map_name VARCHAR(255) NOT NULL,
	begun DATETIME NOT NULL,
    ended DATETIME NOT NULL,
    difficulty INT NOT NULL,
    PRIMARY KEY (match_id),
    FOREIGN KEY (map_name) REFERENCES map(map_name)
);

CREATE TABLE abilities (
	ability_name VARCHAR(255) NOT NULL,
    cost INT NOT NULL,
    PRIMARY KEY (ability_name)
);

CREATE TABLE item_color (
	color_name VARCHAR(255) NOT NULL,
    red FLOAT NOT NULL,
    green FLOAT NOT NULL,
    blue FLOAT NOT NULL,
    PRIMARY KEY (color_name)
);

CREATE TABLE item_type (
	type_value INT NOT NULL,
    PRIMARY KEY (type_value)
);

CREATE TABLE item (
	item_id INT AUTO_INCREMENT NOT NULL,
    item_color VARCHAR(255) NOT NULL,
    item_type INT NOT NULL,
    owner_id VARCHAR(255) NOT NULL,
    aquire_date DATETIME NOT NULL,
    item_name VARCHAR(255) NOT NULL,
    quality FLOAT NOT NULL,
    PRIMARY KEY (item_id),
    FOREIGN KEY (item_color) REFERENCES item_color(color_name),
    FOREIGN KEY (item_type) REFERENCES item_type(type_value),
    FOREIGN KEY (owner_id) REFERENCES players(player_id)
);

-- Relations

-- A composite PK that is two FK is fine in many-many relationships
-- https://stackoverflow.com/questions/10982992/is-it-fine-to-have-foreign-key-as-primary-key
CREATE TABLE played_match (
	player_id VARCHAR(255) NOT NULL,
	match_id INT NOT NULL,
	score INT,
    kills INT,
    deaths INT,
    PRIMARY KEY (match_id, player_id),
	FOREIGN KEY (player_id) REFERENCES players(player_id),
    FOREIGN KEY (match_id) REFERENCES matches(match_id)
);

CREATE TABLE has_learned(
	player_id VARCHAR(255) NOT NULL,
	ability_name VARCHAR(255) NOT NULL,    
    PRIMARY KEY (ability_name, player_id),
    FOREIGN KEY (player_id) REFERENCES players(player_id),
    FOREIGN KEY (ability_name) REFERENCES abilities(ability_name)
);

CREATE TABLE wears(
	player_id VARCHAR(255) NOT NULL,
    item_type INT NOT NULL,
    PRIMARY KEY (item_type, player_id),
    FOREIGN KEY (player_id) REFERENCES players(player_id),
    FOREIGN KEY (item_type) REFERENCES item(item_type)
);

