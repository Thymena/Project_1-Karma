import fs from 'node:fs';
import path from 'node:path';

const projectRoot = 'd:/Unity/Project_1/Assets/Scripts';
const inter = path.join(projectRoot, '.understand-anything/intermediate');
const assembled = JSON.parse(fs.readFileSync(path.join(inter, 'assembled-graph.json'), 'utf8'));
const scan = JSON.parse(fs.readFileSync(path.join(inter, 'scan-result.json'), 'utf8'));
const commit = (() => { try { return require('child_process').execSync('git -C d:/Unity/Project_1 rev-parse HEAD', { encoding: 'utf8' }).trim(); } catch { return 'unknown'; } })();

const fileNodes = assembled.nodes.filter(n => n.type === 'file');
const byFolder = {};
for (const n of fileNodes) {
  const folder = n.filePath.split('/')[0];
  (byFolder[folder] ??= []).push(n.id);
}

const layerDefs = [
  { id: 'layer:player', name: '玩家系统', description: '玩家移动、生命值与受伤交互', folders: ['PlayerMovement', 'PlayerHP', 'PlayerInteractions'] },
  { id: 'layer:gameplay', name: '游戏逻辑', description: '收集品、关卡控制与计时', folders: ['Prize', 'Scene', 'Platform'] },
  { id: 'layer:traps', name: '陷阱与反应', description: '火/冰/刺陷阱及元素碰撞反应', folders: ['Fire', 'Ice', 'Spikes', 'Reactions', 'Tilemap'] },
  { id: 'layer:presentation', name: '表现层', description: '相机、UI、对话与音频', folders: ['Camera', 'UI', 'Text', 'Audio'] },
];

const nodeIdSet = new Set(assembled.nodes.map(n => n.id));
const layers = layerDefs.map(l => ({
  id: l.id,
  name: l.name,
  description: l.description,
  nodeIds: l.folders.flatMap(f => byFolder[f] || []).filter(id => nodeIdSet.has(id)),
})).filter(l => l.nodeIds.length > 0);

const tour = [
  { order: 1, title: '项目概览', description: '从 GameController 了解关卡如何启动收集品与测试流程。', nodeIds: ['file:Scene/GameController.cs'].filter(id => nodeIdSet.has(id)) },
  { order: 2, title: '玩家移动核心', description: 'PlayerMovementRe 实现跳跃、攀墙、冰面滑行等核心平台机制。', nodeIds: ['file:PlayerMovement/PlayerMovementRe.cs'].filter(id => nodeIdSet.has(id)), languageLesson: 'Unity MonoBehaviour 生命周期：Awake/Update/FixedUpdate 分工处理输入与物理。' },
  { order: 3, title: '生命值与伤害', description: 'PlayerHP 与 OnFire/OnSpike/Timer 构成伤害来源。', nodeIds: ['file:PlayerHP/PlayerHP.cs', 'file:PlayerInteractions/OnFire.cs', 'file:PlayerInteractions/OnSpike.cs'].filter(id => nodeIdSet.has(id)) },
  { order: 4, title: '收集与 UI', description: 'CollectibleManager 驱动浆果收集与 UI 计数显示。', nodeIds: ['file:Prize/CollectibleManager.cs', 'file:Prize/Mossberry.cs', 'file:Scene/CollectCountUI.cs'].filter(id => nodeIdSet.has(id)) },
  { order: 5, title: '元素陷阱反应', description: 'Fire/Ice/Spikes 与 Reactions 目录实现元素组合谜题。', nodeIds: ['file:Reactions/FireAndIce.cs', 'file:Fire/FireMove.cs', 'file:Ice/IceMove.cs', 'file:Spikes/SpikesMove.cs'].filter(id => nodeIdSet.has(id)) },
  { order: 6, title: '场景与对话', description: 'SceneControllerScript 负责场景切换，DialogueUIController 处理剧情文本。', nodeIds: ['file:Scene/SceneControllerScript.cs', 'file:Text/DialogueUIController.cs'].filter(id => nodeIdSet.has(id)) },
  { order: 7, title: '相机与视差', description: 'FollowCamera 跟随玩家，Parallax 系统营造 2D 景深。', nodeIds: ['file:Camera/FollowCamera.cs', 'file:Camera/Parallax.cs', 'file:Camera/ParallaxManager.cs'].filter(id => nodeIdSet.has(id)) },
].filter(s => s.nodeIds.length > 0);

const graph = {
  version: '1.0.0',
  project: {
    name: scan.name,
    languages: scan.languages,
    frameworks: scan.frameworks,
    description: scan.description,
    analyzedAt: new Date().toISOString(),
    gitCommitHash: commit,
  },
  nodes: assembled.nodes,
  edges: assembled.edges,
  layers,
  tour,
};

const uaDir = path.join(projectRoot, '.understand-anything');
fs.writeFileSync(path.join(inter, 'assembled-graph.json'), JSON.stringify({ ...assembled, layers, tour }, null, 2));
fs.writeFileSync(path.join(uaDir, 'knowledge-graph.json'), JSON.stringify(graph, null, 2));
fs.writeFileSync(path.join(uaDir, 'meta.json'), JSON.stringify({
  lastAnalyzedAt: graph.project.analyzedAt,
  gitCommitHash: commit,
  version: '1.0.0',
  analyzedFiles: scan.totalFiles,
}, null, 2));

console.log('Saved knowledge-graph.json');
console.log('Nodes:', graph.nodes.length, 'Edges:', graph.edges.length, 'Layers:', layers.length, 'Tour steps:', tour.length);
