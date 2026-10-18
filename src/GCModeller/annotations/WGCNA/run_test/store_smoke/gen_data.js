// 冒烟测试数据生成器：3 个共表达模块 x 100 基因 x 40 样本
// 模块信号 0.85 / 噪声 0.45 => 组内 Pearson 相关约 0.78（> 默认邻接阈值 0.6）
// 注意：Array.from({length:N}, mapFn) 的回调签名是 (element, index)，元素在前！
function mulberry32(a) {
    return function () {
        a |= 0; a = a + 0x6D2B79F5 | 0;
        let t = Math.imul(a ^ a >>> 15, 1 | a);
        t = t + Math.imul(t ^ t >>> 7, 61 | t) ^ t;
        return ((t ^ t >>> 14) >>> 0) / 4294967296;
    };
}

const rnd = mulberry32(20261018);

function gauss() {
    const u1 = rnd() || 1e-9;
    const u2 = rnd();
    return Math.sqrt(-2 * Math.log(u1)) * Math.cos(2 * Math.PI * u2);
}

const fs = require('fs');
const S = 40, M = 3, per = 100;
const prof = [];

for (let m = 0; m < M; m++) {
    prof.push(Array.from({ length: S }, () => gauss()));
}

const rows = [];

for (let m = 0; m < M; m++) {
    for (let i = 0; i < per; i++) {
        const id = 'G' + (m + 1) + '_' + String(i + 1).padStart(3, '0');
        const vals = Array.from({ length: S }, (_, s) => 0.85 * prof[m][s] + 0.45 * gauss());
        rows.push(id + ',' + vals.map(v => v.toFixed(6)).join(','));
    }
}

const header = ',' + Array.from({ length: S }, (_, s) => 'S' + String(s + 1).padStart(2, '0')).join(',');
const dir = 'g:/GCModeller/src/GCModeller/annotations/WGCNA/run_test/store_smoke/';

fs.writeFileSync(dir + 'expr300.csv', header + '\n' + rows.join('\n') + '\n');
fs.writeFileSync(dir + 'expr200.csv', header + '\n' + rows.slice(0, 200).join('\n') + '\n');

const bad = rows.filter(r => r.includes('NaN')).length;
console.log('genes=' + rows.length + ' samples=' + S + ' NaN rows=' + bad);
console.log('sample row: ' + rows[0].substring(0, 90));
