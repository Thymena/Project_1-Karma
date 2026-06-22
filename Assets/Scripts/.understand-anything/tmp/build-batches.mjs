import fs from 'node:fs';
import path from 'node:path';

const projectRoot = 'd:/Unity/Project_1/Assets/Scripts';
const uaDir = path.join(projectRoot, '.understand-anything');
const inter = path.join(uaDir, 'intermediate');

const FILE_SUMMARIES = {
  'Audio/AudioManager.cs': '管理背景音乐播放，在 Start 中获取 AudioSource 并循环播放 clips。',
  'Camera/FollowCamera.cs': '2D 跟随相机，LateUpdate 中平滑跟随玩家 Transform。',
  'Camera/Parallax.cs': '视差滚动背景层，支持无限循环与多轴偏移。',
  'Camera/ParallaxManager.cs': '统一管理多个 Parallax 层，按相机位移驱动各层。',
  'Fire/FireMove.cs': '火焰陷阱移动逻辑，碰撞后切换形态或触发反应。',
  'Ice/IceMove.cs': '冰块陷阱移动，可被推动并在碰撞刺/火时改变状态。',
  'Platform/JumpJumpHappy.cs': '弹跳平台，对玩家施加 burstForce 向上推力。',
  'Platform/MiniGameElevator.cs': '小游戏升降台，上升/返回并配合淡出切换。',
  'Platform/MovingPlatform.cs': '在 pointA 与 pointB 之间往返的移动平台。',
  'Platform/NewLevel.cs': '关卡过渡用移动物体，到达 stopPoint 后停止。',
  'Platform/NextLevelElevator.cs': '通关升降台，上升后加载下一关场景。',
  'PlayerHP/DeathTimesCount.cs': '单例统计玩家死亡次数。',
  'PlayerHP/PersistUI.cs': '跨场景持久化 UI（DontDestroyOnLoad）。',
  'PlayerHP/PlayerHP.cs': '玩家生命值，TakeDamage 处理受伤。',
  'PlayerInteractions/OnFire.cs': '玩家接触火焰时持续扣血并闪烁提示。',
  'PlayerInteractions/OnSpike.cs': '玩家接触尖刺时一次性扣血。',
  'PlayerMovement/PlayerMovementRe.cs': '核心 2D 平台跳跃控制器：移动、跳跃、攀墙、冰面滑行、Coyote Time。',
  'Prize/CollectibleManager.cs': '收集品生成与计时，协调相机聚焦与 GameController。',
  'Prize/Mossberry.cs': '可收集浆果，触发 CollectibleManager 计数与音效。',
  'Reactions/FireAndIce.cs': '火与冰元素碰撞反应，切换物体形态。',
  'Reactions/FireAndIceChildTriggerRelay.cs': '子碰撞体触发器，转发至 FireAndIce 父组件。',
  'Reactions/FireAndSpike.cs': '火与尖刺碰撞反应逻辑。',
  'Reactions/IceAndSpike.cs': '冰与尖刺碰撞反应逻辑。',
  'Reactions/MovableInteractions.cs': '可推动物体的通用移动与停止接口。',
  'Reactions/PushAwayOnPlayerTrigger2D.cs': '玩家触发时将物体推离并锁定移动。',
  'Scene/BerryController.cs': '场景中浆果对象的批量管理。',
  'Scene/CollectCountUI.cs': '显示已收集数量的 UI 文本绑定。',
  'Scene/DisplayCursor.cs': '控制鼠标光标显示/隐藏。',
  'Scene/GameController.cs': '关卡测试开关，控制 gameObjects 激活与 CollectibleManager 生成。',
  'Scene/Menu.cs': '暂停菜单显示与 ESC 切换。',
  'Scene/NormalSceneTransition.cs': '带淡入淡出的场景切换。',
  'Scene/PointerScript.cs': 'UI 指针跟随带 targetTag 的目标物体。',
  'Scene/PointerScriptArrow.cs': '箭头指针，带角度偏移指向目标。',
  'Scene/QuitGame.cs': '退出游戏 Application.Quit。',
  'Scene/ResumeGame.cs': '恢复游戏并关闭暂停菜单。',
  'Scene/SceneControllerScript.cs': '场景过渡单例，BeginTransition 协程切换场景。',
  'Scene/SettingSceneController.cs': '设置场景加载与重载。',
  'Scene/Timer.cs': '倒计时逻辑，超时对玩家扣血。',
  'Spikes/SpikesMove.cs': '尖刺陷阱移动，与火/冰碰撞时改变形态。',
  'Text/DialogueSystemDefaults.cs': '对话系统全局默认参数（打字速度、边距等）。',
  'Text/DialogueUIController.cs': '对话 UI 单例，逐字显示与分段暂停。',
  'Text/OneTimeText.cs': '一次性触发对话，与 DialogueUIController 联动。',
  'Tilemap/TileReactionOnTouch2D.cs': 'Tilemap 碰撞触发的元素反应入口。',
  'UI/ADDisplayUI.cs': '广告/提示 UI 延迟显示。',
  'UI/SpaceBarDisplayUI.cs': '空格键提示 UI，与 ADDisplayUI 配合。',
};

const CLASS_SUMMARIES = {
  AudioManager: 'MonoBehaviour 音频管理器',
  FollowCamera: 'MonoBehaviour 相机跟随',
  Parallax: 'MonoBehaviour 视差层',
  ParallaxManager: 'MonoBehaviour 视差管理器',
  FireMove: 'MonoBehaviour 火焰陷阱',
  IceMove: 'MonoBehaviour 冰块陷阱',
  JumpJumpHappy: 'MonoBehaviour 弹跳平台',
  MiniGameElevator: 'MonoBehaviour 小游戏升降台',
  MovingPlatform: 'MonoBehaviour 移动平台',
  NewLevel: 'MonoBehaviour 关卡过渡物体',
  NextLevelElevator: 'MonoBehaviour 通关升降台',
  DeathTimesCounter: '单例 MonoBehaviour 死亡计数',
  PersistUI: 'MonoBehaviour 持久 UI',
  PlayerHP: 'MonoBehaviour 玩家血量',
  OnFire: 'MonoBehaviour 火焰伤害',
  OnSpike: 'MonoBehaviour 尖刺伤害',
  PlayerMovementRe: 'MonoBehaviour 玩家移动核心',
  CollectibleManager: 'MonoBehaviour 收集品管理',
  Mossberry: 'MonoBehaviour 浆果收集物',
  FireAndIce: 'MonoBehaviour 火冰反应',
  FireAndIceChildTriggerRelay: 'MonoBehaviour 触发转发',
  FireAndSpike: 'MonoBehaviour 火刺反应',
  IceAndSpike: 'MonoBehaviour 冰刺反应',
  MovableInteractions: 'MonoBehaviour 可推动物体',
  PushAwayOnPlayerTrigger2D: 'MonoBehaviour 推离触发器',
  BerryController: 'MonoBehaviour 浆果控制器',
  CollectCountUI: 'MonoBehaviour 收集计数 UI',
  DisplayCursor: 'MonoBehaviour 光标控制',
  GameController: 'MonoBehaviour 关卡控制器',
  Menu: 'MonoBehaviour 暂停菜单',
  NormalSceneTransition: 'MonoBehaviour 场景过渡',
  PointerUI: 'MonoBehaviour UI 指针',
  PointerScriptArrow: 'MonoBehaviour 箭头指针',
  QuitGame: 'MonoBehaviour 退出游戏',
  ResumeGame: 'MonoBehaviour 恢复游戏',
  SceneControllerScript: '单例 MonoBehaviour 场景控制器',
  SettingSceneController: 'MonoBehaviour 设置场景',
  Timer: 'MonoBehaviour 倒计时',
  SpikesMove: 'MonoBehaviour 尖刺移动',
  DialogueSystemDefaults: '静态类 对话默认配置',
  DialogueUIController: '单例 MonoBehaviour 对话 UI',
  OneTimeText: 'MonoBehaviour 一次性对话触发',
  TileReactionOnTouch2D: 'MonoBehaviour Tilemap 反应',
  ADDisplayUI: 'MonoBehaviour 广告 UI',
  SpaceBarDisplayUI: 'MonoBehaviour 空格提示 UI',
};

const LAYER_TAGS = {
  Audio: ['音频'],
  Camera: ['相机', '渲染'],
  Fire: ['陷阱', '元素'],
  Ice: ['陷阱', '元素'],
  Platform: ['平台', '关卡'],
  PlayerHP: ['玩家', 'UI'],
  PlayerInteractions: ['玩家', '伤害'],
  PlayerMovement: ['玩家', '核心'],
  Prize: ['收集', '游戏逻辑'],
  Reactions: ['反应', '物理'],
  Scene: ['场景', '流程'],
  Spikes: ['陷阱'],
  Text: ['对话', 'UI'],
  Tilemap: ['Tilemap', '反应'],
  UI: ['UI'],
};

function complexityFromLines(n) {
  if (n <= 40) return 'simple';
  if (n <= 120) return 'moderate';
  return 'complex';
}

function folderOf(filePath) {
  return filePath.split('/')[0];
}

function buildBatch(batchIndex, extractPath) {
  const extract = JSON.parse(fs.readFileSync(extractPath, 'utf8'));
  const nodes = [];
  const edges = [];
  const edgeSet = new Set();

  function addEdge(source, target, type, weight = 0.8) {
    const key = `${source}|${target}|${type}`;
    if (edgeSet.has(key)) return;
    edgeSet.add(key);
    edges.push({ source, target, type, weight });
  }

  for (const r of extract.results) {
    const fp = r.path;
    const fileId = `file:${fp}`;
    const folder = folderOf(fp);
    const tags = [...(LAYER_TAGS[folder] || ['脚本']), 'csharp', 'unity'];

    nodes.push({
      id: fileId,
      type: 'file',
      name: path.basename(fp),
      filePath: fp,
      summary: FILE_SUMMARIES[fp] || `${folder} 模块脚本`,
      tags,
      complexity: complexityFromLines(r.totalLines),
    });

    for (const cls of r.classes || []) {
      const classId = `class:${fp}:${cls.name}`;
      nodes.push({
        id: classId,
        type: 'class',
        name: cls.name,
        filePath: fp,
        summary: CLASS_SUMMARIES[cls.name] || `类 ${cls.name}`,
        tags: [...tags, '类'],
        complexity: complexityFromLines((cls.endLine || 0) - (cls.startLine || 0)),
      });
      addEdge(fileId, classId, 'contains', 1.0);

      for (const m of cls.methods || []) {
        const fnId = `function:${fp}:${m}`;
        nodes.push({
          id: fnId,
          type: 'function',
          name: m,
          filePath: fp,
          summary: `${cls.name}.${m} 方法`,
          tags: [...tags, '方法'],
          complexity: 'simple',
        });
        addEdge(classId, fnId, 'contains', 1.0);
      }
    }
  }

  // Cross references from known Unity patterns
  const relations = [
    ['class:Scene/GameController.cs:GameController', 'class:Prize/CollectibleManager.cs:CollectibleManager', 'related'],
    ['class:Prize/Mossberry.cs:Mossberry', 'class:Prize/CollectibleManager.cs:CollectibleManager', 'calls'],
    ['class:Scene/CollectCountUI.cs:CollectCountUI', 'class:Prize/CollectibleManager.cs:CollectibleManager', 'related'],
    ['class:Scene/Timer.cs:Timer', 'class:PlayerHP/PlayerHP.cs:PlayerHP', 'calls'],
    ['class:PlayerInteractions/OnFire.cs:OnFire', 'class:PlayerHP/PlayerHP.cs:PlayerHP', 'calls'],
    ['class:PlayerInteractions/OnSpike.cs:OnSpike', 'class:PlayerHP/PlayerHP.cs:PlayerHP', 'calls'],
    ['class:Text/OneTimeText.cs:OneTimeText', 'class:Text/DialogueUIController.cs:DialogueUIController', 'calls'],
    ['class:Reactions/FireAndIceChildTriggerRelay.cs:FireAndIceChildTriggerRelay', 'class:Reactions/FireAndIce.cs:FireAndIce', 'calls'],
    ['class:UI/SpaceBarDisplayUI.cs:SpaceBarDisplayUI', 'class:UI/ADDisplayUI.cs:ADDisplayUI', 'related'],
    ['class:Camera/ParallaxManager.cs:ParallaxManager', 'class:Camera/Parallax.cs:Parallax', 'related'],
    ['class:PlayerMovement/PlayerMovementRe.cs:PlayerMovementRe', 'class:PlayerHP/PlayerHP.cs:PlayerHP', 'related'],
    ['class:Scene/SceneControllerScript.cs:SceneControllerScript', 'class:Scene/NormalSceneTransition.cs:NormalSceneTransition', 'related'],
  ];

  for (const [s, t, type] of relations) {
    if (nodes.some(n => n.id === s) && nodes.some(n => n.id === t)) {
      addEdge(s, t, type, type === 'contains' ? 1.0 : 0.7);
    }
  }

  return { batchIndex, nodes, edges };
}

for (const idx of [1, 2]) {
  const extractPath = path.join(uaDir, 'tmp', `ua-file-extract-results-${idx}.json`);
  const batch = buildBatch(idx, extractPath);
  fs.writeFileSync(path.join(inter, `batch-${idx}.json`), JSON.stringify(batch, null, 2));
  console.log(`batch-${idx}: ${batch.nodes.length} nodes, ${batch.edges.length} edges`);
}
