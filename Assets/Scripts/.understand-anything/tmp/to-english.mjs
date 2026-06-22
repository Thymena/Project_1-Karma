import fs from 'node:fs';
import path from 'node:path';

const uaDir = 'd:/Unity/Project_1/Assets/Scripts/.understand-anything';
const graphPath = path.join(uaDir, 'knowledge-graph.json');
const graph = JSON.parse(fs.readFileSync(graphPath, 'utf8'));

const FILE_EN = {
  'Audio/AudioManager.cs': 'Manages background music; gets AudioSource in Start and loops clips.',
  'Camera/FollowCamera.cs': '2D follow camera that smoothly tracks the player Transform in LateUpdate.',
  'Camera/Parallax.cs': 'Parallax scrolling background layers with infinite scroll and multi-axis offset.',
  'Camera/ParallaxManager.cs': 'Manages multiple Parallax layers driven by camera movement.',
  'Fire/FireMove.cs': 'Fire trap movement; switches form or triggers reactions on collision.',
  'Ice/IceMove.cs': 'Ice block trap; pushable and changes state when hitting spikes or fire.',
  'Platform/JumpJumpHappy.cs': 'Bounce platform applying burstForce upward to the player.',
  'Platform/MiniGameElevator.cs': 'Mini-game elevator with rise/return and fade transition.',
  'Platform/MovingPlatform.cs': 'Platform moving back and forth between pointA and pointB.',
  'Platform/NewLevel.cs': 'Level-transition mover that stops at stopPoint.',
  'Platform/NextLevelElevator.cs': 'Completion elevator that rises then loads the next scene.',
  'PlayerHP/DeathTimesCount.cs': 'Singleton tracking player death count.',
  'PlayerHP/PersistUI.cs': 'Cross-scene persistent UI (DontDestroyOnLoad).',
  'PlayerHP/PlayerHP.cs': 'Player health; TakeDamage handles incoming damage.',
  'PlayerInteractions/OnFire.cs': 'Continuous damage and flash feedback while player touches fire.',
  'PlayerInteractions/OnSpike.cs': 'One-shot damage when player touches spikes.',
  'PlayerMovement/PlayerMovementRe.cs': 'Core 2D platform controller: move, jump, wall cling, ice slide, coyote time.',
  'Prize/CollectibleManager.cs': 'Collectible spawning and timer; coordinates camera focus and GameController.',
  'Prize/Mossberry.cs': 'Collectible berry; triggers CollectibleManager count and SFX.',
  'Reactions/FireAndIce.cs': 'Fire–ice element reaction switching object forms.',
  'Reactions/FireAndIceChildTriggerRelay.cs': 'Child trigger relay forwarding hits to FireAndIce parent.',
  'Reactions/FireAndSpike.cs': 'Fire–spike collision reaction logic.',
  'Reactions/IceAndSpike.cs': 'Ice–spike collision reaction logic.',
  'Reactions/MovableInteractions.cs': 'Generic pushable object move/stop interface.',
  'Reactions/PushAwayOnPlayerTrigger2D.cs': 'Pushes object away and locks movement on player trigger.',
  'Scene/BerryController.cs': 'Batch manager for berry objects in the scene.',
  'Scene/CollectCountUI.cs': 'UI text binding for collected item count.',
  'Scene/DisplayCursor.cs': 'Shows or hides the mouse cursor.',
  'Scene/GameController.cs': 'Level test toggles; activates gameObjects and starts CollectibleManager spawning.',
  'Scene/Menu.cs': 'Pause menu toggled with ESC.',
  'Scene/NormalSceneTransition.cs': 'Scene transition with fade in/out.',
  'Scene/PointerScript.cs': 'UI pointer following objects with targetTag.',
  'Scene/PointerScriptArrow.cs': 'Arrow pointer with angle offset toward target.',
  'Scene/QuitGame.cs': 'Quits the game via Application.Quit.',
  'Scene/ResumeGame.cs': 'Resumes game and closes pause menu.',
  'Scene/SceneControllerScript.cs': 'Scene transition singleton; BeginTransition coroutine.',
  'Scene/SettingSceneController.cs': 'Load and reload settings scene.',
  'Scene/Timer.cs': 'Countdown timer; damages player on timeout.',
  'Spikes/SpikesMove.cs': 'Moving spike trap; morphs when colliding with fire/ice.',
  'Text/DialogueSystemDefaults.cs': 'Global dialogue defaults (type speed, margins, etc.).',
  'Text/DialogueUIController.cs': 'Dialogue UI singleton with typewriter and segment pauses.',
  'Text/OneTimeText.cs': 'One-shot dialogue trigger wired to DialogueUIController.',
  'Tilemap/TileReactionOnTouch2D.cs': 'Tilemap touch entry point for element reactions.',
  'UI/ADDisplayUI.cs': 'Delayed ad/hint UI display.',
  'UI/SpaceBarDisplayUI.cs': 'Space-bar hint UI paired with ADDisplayUI.',
};

const CLASS_EN = {
  AudioManager: 'MonoBehaviour audio manager',
  FollowCamera: 'MonoBehaviour camera follow',
  Parallax: 'MonoBehaviour parallax layer',
  ParallaxManager: 'MonoBehaviour parallax manager',
  FireMove: 'MonoBehaviour fire trap',
  IceMove: 'MonoBehaviour ice trap',
  JumpJumpHappy: 'MonoBehaviour bounce platform',
  MiniGameElevator: 'MonoBehaviour mini-game elevator',
  MovingPlatform: 'MonoBehaviour moving platform',
  NewLevel: 'MonoBehaviour level transition object',
  NextLevelElevator: 'MonoBehaviour completion elevator',
  DeathTimesCounter: 'Singleton MonoBehaviour death counter',
  PersistUI: 'MonoBehaviour persistent UI',
  PlayerHP: 'MonoBehaviour player health',
  OnFire: 'MonoBehaviour fire damage',
  OnSpike: 'MonoBehaviour spike damage',
  PlayerMovementRe: 'MonoBehaviour core player movement',
  CollectibleManager: 'MonoBehaviour collectible manager',
  Mossberry: 'MonoBehaviour berry collectible',
  FireAndIce: 'MonoBehaviour fire–ice reaction',
  FireAndIceChildTriggerRelay: 'MonoBehaviour trigger relay',
  FireAndSpike: 'MonoBehaviour fire–spike reaction',
  IceAndSpike: 'MonoBehaviour ice–spike reaction',
  MovableInteractions: 'MonoBehaviour pushable object',
  PushAwayOnPlayerTrigger2D: 'MonoBehaviour push-away trigger',
  BerryController: 'MonoBehaviour berry controller',
  CollectCountUI: 'MonoBehaviour collect count UI',
  DisplayCursor: 'MonoBehaviour cursor control',
  GameController: 'MonoBehaviour level controller',
  Menu: 'MonoBehaviour pause menu',
  NormalSceneTransition: 'MonoBehaviour scene transition',
  PointerUI: 'MonoBehaviour UI pointer',
  PointerScriptArrow: 'MonoBehaviour arrow pointer',
  QuitGame: 'MonoBehaviour quit game',
  ResumeGame: 'MonoBehaviour resume game',
  SceneControllerScript: 'Singleton MonoBehaviour scene controller',
  SettingSceneController: 'MonoBehaviour settings scene',
  Timer: 'MonoBehaviour countdown timer',
  SpikesMove: 'MonoBehaviour moving spikes',
  DialogueSystemDefaults: 'Static class dialogue defaults',
  DialogueUIController: 'Singleton MonoBehaviour dialogue UI',
  OneTimeText: 'MonoBehaviour one-shot dialogue trigger',
  TileReactionOnTouch2D: 'MonoBehaviour tilemap reaction',
  ADDisplayUI: 'MonoBehaviour ad UI',
  SpaceBarDisplayUI: 'MonoBehaviour space-bar hint UI',
};

const TAG_MAP = {
  '音频': 'audio', '相机': 'camera', '渲染': 'rendering', '陷阱': 'trap', '元素': 'element',
  '平台': 'platform', '关卡': 'level', '玩家': 'player', '伤害': 'damage', '核心': 'core',
  '收集': 'collectible', '游戏逻辑': 'gameplay', '反应': 'reaction', '物理': 'physics',
  '场景': 'scene', '流程': 'flow', '对话': 'dialogue', 'UI': 'ui', 'Tilemap': 'tilemap',
  '脚本': 'script', '类': 'class', '方法': 'method',
};

graph.project.description = 'Unity 2D platformer with player movement, fire/ice/spike reactions, dialogue, collectibles, scene flow, and UI.';

for (const n of graph.nodes) {
  if (n.type === 'file' && FILE_EN[n.filePath]) n.summary = FILE_EN[n.filePath];
  else if (n.type === 'class' && CLASS_EN[n.name]) n.summary = CLASS_EN[n.name];
  else if (n.type === 'function') {
    const cls = n.id.split(':')[2]?.split(':')[0] || n.name;
    n.summary = `${cls}.${n.name} method`;
  }
  if (Array.isArray(n.tags)) {
    n.tags = n.tags.map(t => TAG_MAP[t] || t);
  }
}

const LAYER_EN = {
  'layer:player': { name: 'Player Systems', description: 'Movement, health, and damage interactions' },
  'layer:gameplay': { name: 'Gameplay', description: 'Collectibles, level control, and platforms' },
  'layer:traps': { name: 'Traps & Reactions', description: 'Fire/ice/spike traps and element combos' },
  'layer:presentation': { name: 'Presentation', description: 'Camera, UI, dialogue, and audio' },
};

const TOUR_EN = [
  { order: 1, title: 'Project Overview', description: 'Start with GameController to see how levels boot collectibles and test flow.' },
  { order: 2, title: 'Player Movement', description: 'PlayerMovementRe implements jump, wall cling, ice slide, and coyote time.', languageLesson: 'Unity MonoBehaviour lifecycle: Awake/Update/FixedUpdate split input vs physics.' },
  { order: 3, title: 'Health & Damage', description: 'PlayerHP plus OnFire/OnSpike/Timer define damage sources.' },
  { order: 4, title: 'Collectibles & UI', description: 'CollectibleManager drives berry pickup and count UI.' },
  { order: 5, title: 'Element Traps', description: 'Fire/Ice/Spikes and Reactions implement puzzle combos.' },
  { order: 6, title: 'Scenes & Dialogue', description: 'SceneControllerScript handles transitions; DialogueUIController shows story text.' },
  { order: 7, title: 'Camera & Parallax', description: 'FollowCamera tracks the player; Parallax adds 2D depth.' },
];

for (const layer of graph.layers || []) {
  const en = LAYER_EN[layer.id];
  if (en) { layer.name = en.name; layer.description = en.description; }
}

for (const step of graph.tour || []) {
  const en = TOUR_EN.find(t => t.order === step.order);
  if (en) {
    step.title = en.title;
    step.description = en.description;
    if (en.languageLesson) step.languageLesson = en.languageLesson;
  }
}

fs.writeFileSync(graphPath, JSON.stringify(graph, null, 2));
fs.writeFileSync(path.join(uaDir, 'config.json'), JSON.stringify({ outputLanguage: 'en', autoUpdate: false }, null, 2));
console.log('Converted knowledge graph to English summaries');
