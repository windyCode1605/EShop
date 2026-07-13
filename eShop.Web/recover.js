const fs = require('fs');
const path = require('path');

const logPath = 'C:/Users/nguyennn/.gemini/antigravity-ide/brain/012f7334-815d-4727-9d80-d17616975e9c/.system_generated/logs/transcript_full.jsonl';
const lines = fs.readFileSync(logPath, 'utf8').split('\n');

const filesToRecover = [
  'admin-categories.component.ts',
  'category-form.component.ts',
  'admin-products.component.ts',
  'product-form.component.ts'
];

const recoveredContent = {};

for (const line of lines) {
  if (!line) continue;
  try {
    const data = JSON.parse(line);
    // Look at tool calls
    if (data.tool_calls) {
      for (const call of data.tool_calls) {
        if (call.name === 'default_api:write_to_file') {
          const target = call.arguments.TargetFile;
          const content = call.arguments.CodeContent;
          if (target && content) {
            for (const f of filesToRecover) {
              if (target.endsWith(f)) {
                recoveredContent[f] = content;
              }
            }
          }
        }
      }
    }
  } catch (e) {}
}

for (const [f, content] of Object.entries(recoveredContent)) {
  fs.writeFileSync(f + '.recovered', content);
  console.log('Recovered', f);
}
