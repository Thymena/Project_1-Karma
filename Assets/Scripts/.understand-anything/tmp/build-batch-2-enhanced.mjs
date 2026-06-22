import fs from 'node:fs';
import path from 'node:path';

const uaDir = 'd:/Unity/Project_1/Assets/Scripts/.understand-anything';
const inter = path.join(uaDir, 'intermediate');

const FILE_SUMMARIES = {
  'Scene/BerryController.cs': '启动时批量禁用 berries 数组中的浆果 GameObject。',
  'Scene/CollectCountUI.cs': '订阅 CollectibleManager.OnCountChanged，用 TextMeshPro 显示收集进度 x/5。',
  'Scene/DisplayCursor.cs': 'Start 时解锁并显示鼠标光标，用于菜单/设置场景。',
  'Scene/GameController.cs': '通过 startTest/pastTest 开关控制关卡物体显隐并调用 CollectibleManager.BeginSpawning。',
  'Scene/Menu.cs': 'ESC 打开暂停菜单，Time.timeScale=0 并显示光标。',
  'Scene/NormalSceneTransition.cs': '玩家触发器进入后调用 SceneControllerScript.instance.BeginTransition 切换下一关。',
  'Scene/PointerScript.cs': 'Canvas UI 指针（PointerUI）朝向激活的 Berry 标签物体并旋转指示方向。',
  'Scene/PointerScriptArrow.cs': '世界空间箭头指针，按 angleOffset 旋转指向激活的 Berry。',
  'Scene/QuitGame.cs': 'Quit 方法：编辑器停止播放，构建版 Application.Quit。',
  'Scene/ResumeGame.cs': 'Resume 关闭暂停菜单、恢复 timeScale 并锁定隐藏光标。',
  'Scene/SceneControllerScript.cs': 'DontDestroyOnLoad 单例，协程驱动 Animator 淡出并 LoadSceneAsync 下一关。',
  'Scene/SettingSceneController.cs': '设置菜单用 LoadScene/ReloadScene 加载或重载场景。',
  'Scene/Timer.cs': '订阅 CollectibleManager 倒计时，UI 显示 MM:SS，归零时对 PlayerHP 扣血。',
  'Spikes/SpikesMove.cs': '玩家在触发区内按 F 推动 movedObject，碰撞 Floor/Ice/Fire 时停止并销毁或切换形态。',
  'Text/DialogueSystemDefaults.cs': '对话与 OneTimeText 的静态默认常量（打字间隔、边距、玩家 Tag 等）。',
  'Text/DialogueUIController.cs': '单例 DialogueUIController.Instance，打字机效果播放多段对话并通知 OneTimeText 完成。',
  'Text/OneTimeText.cs': '2D 触发器延迟后调用 DialogueUIController.TryPlayDialogue，支持 PlayerPrefs 一次性存档。',
  'Tilemap/TileReactionOnTouch2D.cs': '碰撞/触发 Tilemap 时擦除单格瓦片、Instantiate 替换物并销毁或变形自身。',
  'UI/ADDisplayUI.cs': '延迟显示 ad 提示，按 A/D 关闭并置 isDisplayed=true。',
  'UI/SpaceBarDisplayUI.cs': 'ADDisplayUI 显示后延迟展示空格提示，按 Space 隐藏。',
};

const CLASS_SUMMARIES = {
  BerryController: 'MonoBehaviour：管理浆果数组初始禁用状态。',
  CollectCountUI: 'MonoBehaviour：绑定 CollectibleManager 事件更新 TMP 文本。',
  DisplayCursor: 'MonoBehaviour：菜单场景光标可见性。',
  GameController: 'MonoBehaviour：关卡测试流程与收集品生成开关。',
  Menu: 'MonoBehaviour：ESC 暂停与 pauseMenu 显隐。',
  NormalSceneTransition: 'MonoBehaviour：2D 触发器驱动场景淡入淡出切换。',
  PointerUI: 'MonoBehaviour：Screen Space Overlay 指针朝向 Berry。',
  PointerScriptArrow: 'MonoBehaviour：世界空间箭头指向 Berry。',
  QuitGame: 'MonoBehaviour：退出游戏按钮逻辑。',
  ResumeGame: 'MonoBehaviour：从暂停恢复游戏状态。',
  SceneControllerScript: '单例 MonoBehaviour：跨场景场景过渡与 Animator 联动。',
  SettingSceneController: 'MonoBehaviour：设置界面场景跳转。',
  Timer: 'MonoBehaviour：收集挑战倒计时 UI 与超时扣血。',
  SpikesMove: 'MonoBehaviour：可推动尖刺块，与 Ice/Fire 标签碰撞反应。',
  DialogueSystemDefaults: '静态类：对话系统默认配置常量。',
  DialogueUIController: '单例 MonoBehaviour：对话面板打字机 UI 控制器。',
  OneTimeText: 'MonoBehaviour：区域触发一次性/可重复对话。',
  TileReactionOnTouch2D: 'MonoBehaviour：Tilemap 单格消除与生成替换物。',
  ADDisplayUI: 'MonoBehaviour：A/D 操作提示 UI。',
  SpaceBarDisplayUI: 'MonoBehaviour：空格键提示 UI，依赖 ADDisplayUI。',
};

const METHOD_SUMMARIES = {
  'Scene/GameController.cs:Update': 'startTest 时激活 gameObjects 并 BeginSpawning；pastTest 时全部禁用。',
  'Scene/CollectCountUI.cs:Start': '订阅 OnCountChanged 并初始化 countText 为当前收集数。',
  'Scene/CollectCountUI.cs:UpdateCountUI': '将 count 格式化为 "n/5" 写入 TextMeshProUGUI。',
  'Scene/NormalSceneTransition.cs:TriggerRoutine': '等待一帧后调用 SceneControllerScript.instance.BeginTransition。',
  'Scene/SceneControllerScript.cs:Awake': '单例初始化，DontDestroyOnLoad 或销毁重复实例。',
  'Scene/SceneControllerScript.cs:BeginTransition': '启动 TransitionCoroutine 执行淡出与异步加载下一 buildIndex 场景。',
  'Scene/SceneControllerScript.cs:TransitionCoroutine': 'Animator End 触发、WaitForSeconds、LoadSceneAsync、新场景 Start 淡入。',
  'Scene/Timer.cs:UpdateTimerUI': 'time<=0 时 GetComponent<PlayerHP> 扣 health；否则更新 MM:SS 文本。',
  'Scene/PointerScript.cs:GetActiveBerry': 'FindGameObjectsWithTag(targetTag) 返回第一个 activeInHierarchy 的 Transform。',
  'Scene/PointerScriptArrow.cs:GetActiveBerry': '与 PointerUI 相同逻辑查找激活 Berry。',
  'Spikes/SpikesMove.cs:MoveAwayCoroutine': '按玩家相对方向沿纯 X/Y 移动直至 hit 或碰撞 Floor/Ice/Fire。',
  'Spikes/SpikesMove.cs:StopMovement': '外部调用：置 hit=true 停止移动协程。',
  'Text/DialogueUIController.cs:TryPlayDialogue': '若未播放且 lines 有效则启动 PlayDialogueSession 协程。',
  'Text/DialogueUIController.cs:Awake': '设置 Instance、分离 Overlay Canvas、配置 TMP 自适应与默认参数。',
  'Text/OneTimeText.cs:OnTriggerEnter2D': '玩家进入后延迟触发，检查 Instance 与 IsPlaying 再 TryPlayDialogue。',
  'Text/OneTimeText.cs:NotifyDialogueFinished': '非 allowRepeat 时写入 PlayerPrefs 标记已完成。',
  'Tilemap/TileReactionOnTouch2D.cs:TryReact': 'WorldToCell 擦除单格、Instantiate spawnPrefab，销毁或改 sprite/tag/layer。',
  'UI/SpaceBarDisplayUI.cs:Update': 'adDisplayUI.isDisplayed 后计时显示 spaceBar，Space 键关闭。',
};

const LAYER_TAGS = {
  Scene: ['场景', '流程'],
  Spikes: ['陷阱'],
  Text: ['对话', 'UI'],
  Tilemap: ['Tilemap', '反应'],
  UI: ['UI'],
};

const IN_BATCH_RELATIONS = [
  ['class:Scene/NormalSceneTransition.cs:NormalSceneTransition', 'class:Scene/SceneControllerScript.cs:SceneControllerScript', 'calls'],
  ['function:Scene/NormalSceneTransition.cs:TriggerRoutine', 'function:Scene/SceneControllerScript.cs:BeginTransition', 'calls'],
  ['class:Text/DialogueUIController.cs:DialogueUIController', 'class:Text/DialogueSystemDefaults.cs:DialogueSystemDefaults', 'related'],
  ['class:Text/OneTimeText.cs:OneTimeText', 'class:Text/DialogueSystemDefaults.cs:DialogueSystemDefaults', 'related'],
  ['class:Text/OneTimeText.cs:OneTimeText', 'class:Text/DialogueUIController.cs:DialogueUIController', 'calls'],
  ['function:Text/OneTimeText.cs:OnTriggerEnter2D', 'function:Text/DialogueUIController.cs:TryPlayDialogue', 'calls'],
  ['function:Text/DialogueUIController.cs:PlayDialogueSession', 'function:Text/OneTimeText.cs:NotifyDialogueFinished', 'calls'],
  ['class:Scene/PointerScript.cs:PointerUI', 'class:Scene/PointerScriptArrow.cs:PointerScriptArrow', 'related'],
  ['class:Scene/Menu.cs:Menu', 'class:Scene/ResumeGame.cs:ResumeGame', 'related'],
  ['class:UI/SpaceBarDisplayUI.cs:SpaceBarDisplayUI', 'class:UI/ADDisplayUI.cs:ADDisplayUI', 'related'],
  ['class:Scene/SceneControllerScript.cs:SceneControllerScript', 'class:Scene/NormalSceneTransition.cs:NormalSceneTransition', 'related'],
];

/** Cross-batch class refs (target may exist in batch-1 import) */
const EXTERNAL_RELATIONS = [
  ['class:Scene/GameController.cs:GameController', 'class:Prize/CollectibleManager.cs:CollectibleManager', 'calls'],
  ['class:Scene/CollectCountUI.cs:CollectCountUI', 'class:Prize/CollectibleManager.cs:CollectibleManager', 'related'],
  ['class:Scene/Timer.cs:Timer', 'class:Prize/CollectibleManager.cs:CollectibleManager', 'related'],
  ['class:Scene/Timer.cs:Timer', 'class:PlayerHP/PlayerHP.cs:PlayerHP', 'calls'],
  ['class:Prize/CollectibleManager.cs:CollectibleManager', 'class:Scene/GameController.cs:GameController', 'related'],
];

function complexityFromLines(n) {
  if (n <= 40) return 'simple';
  if (n <= 120) return 'moderate';
  return 'complex';
}

function folderOf(filePath) {
  return filePath.split('/')[0];
}

const extract = JSON.parse(
  fs.readFileSync(path.join(uaDir, 'tmp', 'ua-file-extract-results-2.json'), 'utf8')
);

const nodes = [];
const edges = [];
const edgeSet = new Set();
const nodeIds = new Set();

function addEdge(source, target, type, weight = 0.7) {
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
  nodeIds.add(fileId);

  for (const cls of r.classes || []) {
    const classId = `class:${fp}:${cls.name}`;
    const lineSpan = (cls.endLine || 0) - (cls.startLine || 0);
    nodes.push({
      id: classId,
      type: 'class',
      name: cls.name,
      filePath: fp,
      summary: CLASS_SUMMARIES[cls.name] || `类 ${cls.name}`,
      tags: [...tags, '类'],
      complexity: complexityFromLines(lineSpan),
    });
    nodeIds.add(classId);
    addEdge(fileId, classId, 'contains', 1.0);

    for (const m of cls.methods || []) {
      const fnId = `function:${fp}:${m}`;
      const methodKey = `${fp}:${m}`;
      nodes.push({
        id: fnId,
        type: 'function',
        name: m,
        filePath: fp,
        summary: METHOD_SUMMARIES[methodKey] || `${cls.name}.${m} 方法`,
        tags: [...tags, '方法'],
        complexity: 'simple',
      });
      nodeIds.add(fnId);
      addEdge(classId, fnId, 'contains', 1.0);
    }
  }
}

for (const [s, t, type] of IN_BATCH_RELATIONS) {
  if (nodeIds.has(s) && nodeIds.has(t)) addEdge(s, t, type, 0.7);
}

for (const [s, t, type] of EXTERNAL_RELATIONS) {
  if (nodeIds.has(s)) addEdge(s, t, type, 0.7);
}

const batch = { batchIndex: 2, nodes, edges };
fs.writeFileSync(path.join(inter, 'batch-2.json'), JSON.stringify(batch, null, 2));
console.log(`batch-2: ${nodes.length} nodes, ${edges.length} edges`);
