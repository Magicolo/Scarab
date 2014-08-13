import UnityEngine
import System.Collections.Generic

enum ColorMode:
	Normal
	Random

def SpawnInfoText(text as string, owner as GameObject, color as Color, colorMode as ColorMode, size as single) as InfoText:
	newInfoText as GameObject = hObjectPool.Instance.Spawn(References.InfoTextID, owner.transform.position, Quaternion.identity)
	infoText as InfoText = newInfoText.GetComponent(InfoText)
	infoText.owner = owner
	infoText.textMesh.text = text
	infoText.textMesh.color = color
	infoText.colorMode = colorMode
	infoText.transform.localScale = Vector2(size, size)
	
	return infoText

def SpawnInfoText(text as string, owner as GameObject) as InfoText:
	return SpawnInfoText(text, owner, Color.white, ColorMode.Normal, 1)

def SpawnInfoText(text as string, owner as GameObject, color as Color) as InfoText:
	return SpawnInfoText(text, owner, color, ColorMode.Normal, 1)
	
def SpawnInfoText(text as string, owner as GameObject, color as Color, colorMode as ColorMode) as InfoText:
	return SpawnInfoText(text, owner, color, colorMode, 1)

def GetBarkDict() as Dictionary[of string, single]:
	barkDict as Dictionary[of string, single] = Dictionary[of string, single]()
	barkDict["Happy birthday, ol' friend!"] = 200
	barkDict["Happy birthday!"] = 200
	barkDict["Cheers to you!"] = 200
	barkDict["Hey hey hey! Happy birthday my friend."] = 200
	barkDict["Birthday suprise party!"] = 200
	barkDict["Don't kill us! We're your friends!"] = 150
	barkDict["What the hell are you doing?"] = 150
	barkDict["Another wonderful year behind you."] = 150
	barkDict["Enjoy this very special day!"] = 150
	barkDict["Isn't this your best day of the year?"] = 100
	barkDict["You ain't looking like you're gonna kick the bucket anytime soon!"] = 100
	barkDict["Getting rid of us won't rid you of your fear of getting old."] = 100
	barkDict["Growing older is a beautiful thing."] = 100
	barkDict["You look tormented my friend. Do you want to talk about it?"] = 100
	barkDict["I hope the best for you this year."] = 100
	barkDict["You look goooooood son."] = 100
	barkDict["You shouldn't fight what you are."] = 100
	barkDict["It's not about losing years to live, it's about cumulating accomplishments."] = 100
	barkDict["Keep it up and you'll make it to 100."] = 50
	barkDict["Onto the next stage of life!"] = 50
	barkDict["I always thought you'd make it this far."] = 25
	barkDict["Hello young fellow."] = 10
	barkDict["I'm an old man and I wear a fancy robe."] = 10
	barkDict["I know nothing."] = 10
	barkDict["I am Error."] = 5
	return barkDict

def SpawnRandomBark(owner as Character):
	newBark as GameObject = hObjectPool.Instance.Spawn(References.BarkID, owner.transform.position, Quaternion.identity)
	bark as Bark = newBark.GetComponent[of Bark]()
	bark.currentText = HF.WeightedRandom[of string](References.BarkDict)
	bark.owner = owner
	
def SpawnRandomBonusItem(position as Vector2) as BonusItem:
	SpawnRandomBonusItem(position, 1)
	
def SpawnRandomBonusItem(position as Vector2, bias as single) as BonusItem:
	randomID = References.BonusItems.ItemIDs[Random.Range(0, References.BonusItems.ItemIDs.Count)]
	newDrop as GameObject = hObjectPool.Instance.Spawn(randomID, GetUnoccupiedArea(position, 1), Quaternion.identity)
	bonusItem as BonusItem = newDrop.GetComponent(BonusItem)
	bonusItem.bias = bias
	return bonusItem
	
def GetUnoccupiedPoint(position as Vector2) as Vector2:
	hitTrigger as bool = Physics2D.raycastsHitTriggers
	Physics2D.raycastsHitTriggers = false
	while Physics2D.OverlapPoint(position):
		position.x += Random.value - 0.5
		position.y += Random.value - 0.5
	Physics2D.raycastsHitTriggers = hitTrigger
	return position
	
def GetUnoccupiedArea(position as Vector2, areaSize as single) as Vector2:
	return GetUnoccupiedArea(position, Vector2(areaSize, areaSize))
	
def GetUnoccupiedArea(position as Vector2, areaSize as Vector2) as Vector2:
	hitTrigger as bool = Physics2D.raycastsHitTriggers
	Physics2D.raycastsHitTriggers = false
	while Physics2D.OverlapArea(Vector2(position.x - areaSize.x / 2, position.y - areaSize.y / 2), Vector2(position.x + areaSize.x / 2, position.y + areaSize.y / 2)):
		position.x += (Random.value - 0.5) * areaSize.x
		position.y += (Random.value - 0.5) * areaSize.y
	Physics2D.raycastsHitTriggers = hitTrigger
	return position
	
	