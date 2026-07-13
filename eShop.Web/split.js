const fs = require('fs');

function splitComponent(tsPath) {
  const content = fs.readFileSync(tsPath, 'utf8');

  // We find "styles: [`" and template: `
  const styleStart = content.indexOf('styles: [`');
  const templateStart = content.indexOf('template: `');
  
  if (styleStart === -1 || templateStart === -1) {
    console.log('Could not find styles or template in', tsPath);
    return;
  }
  
  const styleEnd = content.indexOf('`],', styleStart);
  if (styleEnd === -1) return;
  const styleContent = content.substring(styleStart + 10, styleEnd).trim();
  
  const templateEnd = content.lastIndexOf('`\n})');
  if (templateEnd === -1) return;
  const templateContent = content.substring(templateStart + 11, templateEnd).trim();
  
  const basePath = tsPath.replace('.component.ts', '');
  fs.writeFileSync(basePath + '.component.scss', styleContent);
  fs.writeFileSync(basePath + '.component.html', templateContent);
  
  let newContent = content.substring(0, styleStart);
  newContent += `styleUrl: './${basePath.split('/').pop()}.component.scss',\n  templateUrl: './${basePath.split('/').pop()}.component.html'\n})`;
  
  fs.writeFileSync(tsPath, newContent);
  console.log('Successfully split', tsPath);
}

const files = [
  'd:/PROJECT/EShop/eShop.Web/src/app/modules/admin/pages/admin-categories/admin-categories.component.ts',
  'd:/PROJECT/EShop/eShop.Web/src/app/modules/admin/pages/admin-categories/category-form/category-form.component.ts',
  'd:/PROJECT/EShop/eShop.Web/src/app/modules/admin/pages/admin-products/admin-products.component.ts',
  'd:/PROJECT/EShop/eShop.Web/src/app/modules/admin/pages/admin-products/product-form/product-form.component.ts'
];

files.forEach(splitComponent);
