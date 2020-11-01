-- -------------------------------------
-- Skript to track TMDC unit losses
-- by Pixinger
-- Version 1.0
-- -------------------------------------
pixLogger = mist.Logger:new("[[TMDC]]", "info") --warning, error, info, none/off
pixLogger:info("[INIT]")


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
			local position = unit:getPosition();
			local positionStr = "(" .. position.p.x .. "|" .. position.p.y .. "|" ..  position.p.z .. ")"		
			local groupName = unit:getGroup()
			if (groupName) then
				groupName = groupName:getName()
			else
				groupName = "nil"
			end
			pixLogger:info("[SUM][UNI]" .. groupName .. "|" .. unit:getName() .. "|" .. unit:getNumber() .. "|" .. unit:getID() .. "|" .. unit:getTypeName() .. "|" .. positionStr .. "|" .. unit:getLife())
		
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
	
	if event.id == world.event.S_EVENT_CRASH and event.initiator then
		local position = event.initiator:getPosition();
		local positionStr = "(" .. position.p.x .. "|" .. position.p.y .. "|" ..  position.p.z .. ")"
		local groupName = event.initiator:getGroup()
		if (groupName) then
			groupName = groupName:getName()
		else
			groupName = "nil"
		end
		pixLogger:info("[CRASH]" .. groupName .. "|" .. event.initiator:getName() .. "|" .. event.initiator:getNumber() .. "|" .. event.initiator:getID() .. "|" .. event.initiator:getTypeName() .. "|" .. positionStr)
	end

	if event.id == world.event.S_EVENT_DEAD and event.initiator then
		pixLogger:info("[DEBUG]" .. mist.utils.tableShow(event))
		local position = event.initiator:getPosition();
		local positionStr = "(" .. position.p.x .. "|" .. position.p.y .. "|" ..  position.p.z .. ")"
		local groupName = event.initiator:getGroup()
		if (groupName) then
			groupName = groupName:getName()
		else
			groupName = "nil"
		end
		pixLogger:info("[DEAD]" .. groupName .. "|" .. event.initiator:getName() .. "|" .. event.initiator:getNumber() .. "|" .. event.initiator:getID() .. "|" .. event.initiator:getTypeName() .. "|" .. positionStr)
    end

    if event.id == world.event.S_EVENT_KILL and event.target and event.initiator then
		local position = event.initiator:getPosition();
		local positionStr = "(" .. position.p.x .. "|" .. position.p.y .. "|" ..  position.p.z .. ")"
		local groupName = event.target:getGroup()
		if (groupName) then
			groupName = groupName:getName()
		else
			groupName = "nil"
		end
		pixLogger:info("[KILL]" .. groupName .. "|" .. event.target:getName() .. "|" .. event.target:getNumber() .. "|" .. event.target:getID() .. "|" .. event.target:getTypeName() .. "|" .. positionStr .. "|" .. event.initiator:getName())
    end

    if event.id == world.event.S_EVENT_SHOT and event.weapon and event.initiator then
		local position = event.initiator:getPosition();
		local positionStr = "(" .. position.p.x .. "|" .. position.p.y .. "|" ..  position.p.z .. ")"
		local groupName = event.initiator:getGroup()
		if (groupName) then
			groupName = groupName:getName()
		else
			groupName = "nil"
		end
		pixLogger:info("[SHOT]" .. groupName .. "|" .. event.initiator:getName() .. "|" .. event.initiator:getNumber() .. "|" .. event.initiator:getID() .. "|" .. event.initiator:getTypeName() .. "|" .. positionStr .. "|" .. event.weapon:getTypeName())
    end

    if event.id == world.event.S_EVENT_BASE_CAPTURED and event.place then
		pixLogger:info("[BASE_CAPTURED]" .. event.place:getCoalition() .. "|" ..  event.place:getName())
    end

end

mist.addEventHandler(onEvent)