-- -------------------------------------
-- Skript to track TMDC unit losses
-- by Pixinger
-- Version 1.0
-- -------------------------------------
pixLogger = mist.Logger:new("[[TMDC]]", "info") --warning, error, info, none/off
pixLogger:info("[INIT]")

local function ComposeUnitString(unit)
	if (unit) then
		local position = unit:getPosition();
		local positionStr = "(" .. position.p.x .. "|" .. position.p.y .. "|" ..  position.p.z .. ")"
		
		local groupName = ""
		if (unit:getGroup()) then
			groupName = unit:getGroup():getName()
		end

		return groupName .. "|" .. unit:getName() .. "|" .. unit:getCoalition() .. "|" .. Unit.getCategory(unit) .. "|" .. unit:getID() .. "|" .. unit:getTypeName() .. "|" .. positionStr
	else
		return  "||||||"
	end
end

function dumpPix(o)
	if (0 == nil) then
		return '<nil>'
	else
		if type(o) == 'table' then
			local s = '{ '
			for k,v in pairs(o) do
				if type(k) ~= 'number' then k = '"'..k..'"' end
				s = s .. '['..k..'] = ' .. dumpPix(v) .. ','
			end
			return s .. '} '
		else
			return tostring(o)
		end
	end	
end

function LogUnitPositions(coalitionId) -- 0=Neutral, 1=red, 2=blue
	pixLogger:info("[SUM][SID]" .. coalitionId)
	for iGroup, group in pairs(coalition.getGroups(coalitionId)) do
		pixLogger:info("[SUM][GRP]" .. group:getName())
	
		for iUnit, unit in pairs(group:getUnits()) do
			pixLogger:info("[SUM][UNI]" .. ComposeUnitString(unit) .. "|" .. unit:getLife())
		end		
	end
end

local function onEvent(event)
	if event.id == world.event.S_EVENT_MISSION_START then
		pixLogger:info("[START]")
	end
	if event.id == world.event.S_EVENT_MISSION_END then
		LogUnitPositions(1)
		LogUnitPositions(2)
		pixLogger:info("[END]")
	end
	
	-- Für Luftfahrzeuge
	if event.id == world.event.S_EVENT_CRASH and event.initiator then
		pixLogger:info("[CRSH]" .. event.time .. "|" .. ComposeUnitString(event.initiator))
	end

	-- Für Bodenfahrzeuge
	if event.id == world.event.S_EVENT_DEAD and event.initiator then
		--pixLogger:info("[DEBUG]" .. mist.utils.tableShow(event))
		pixLogger:info("[DEAD]" .. event.time .. "|" .. ComposeUnitString(event.initiator))
    end
	
	--if event.id == world.event.S_EVENT_PILOT_DEAD and event.initiator then
	--	pixLogger:info("[PDED]" .. event.time .. "|" .. ComposeUnitString(event.initiator))
    --end

	-- Für eventuelle Details zu DEAD/CRSH
    if event.id == world.event.S_EVENT_KILL and event.target and event.initiator then
		-- event.weapon could nil
		local weaponname = ""
		if (event.weapon) then 
			weaponname = event.weapon:getTypeName()
		end
		
		pixLogger:info("[KILR]" .. event.time .. "|" .. ComposeUnitString(event.initiator) .. "|[TARG]" .. event.time .. "|" .. ComposeUnitString(event.target) .. "|" .. weaponname)
    end

    if event.id == world.event.S_EVENT_SHOT and event.weapon and event.initiator then
		pixLogger:info("[SHOT]" .. event.time .. "|" .. ComposeUnitString(event.initiator) .. "|" .. event.weapon:getTypeName())
    end

    if event.id == world.event.S_EVENT_BASE_CAPTURED and event.place then
		pixLogger:info("[BASE]" .. event.time .. "|" .. event.place:getCoalition() .. "|" ..  event.place:getName())
    end

end

mist.addEventHandler(onEvent)