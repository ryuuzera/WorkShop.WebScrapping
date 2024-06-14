const fs = require('fs');
const path = require('path');
const sharp = require('sharp');

async function convertSVGtoPNG(svgPath, pngPath) {
  try {
    await sharp(svgPath).png().toFile(pngPath);
    console.log(`Converted: ${svgPath} to ${pngPath}`);
  } catch (error) {
    console.error(`Error converting ${svgPath}: ${error.message}`);
  }
}

async function main() {
  const svgDir = './out/svg';
  const pngDir = './out/png';

  // Garanta que os diretórios de saída existam
  await fs.promises.mkdir(svgDir, { recursive: true });
  await fs.promises.mkdir(pngDir, { recursive: true });

  // Leia os arquivos SVG do diretório svgDir
  fs.readdir(svgDir, (err, files) => {
    if (err) {
      console.error(`Error reading directory: ${err.message}`);
      return;
    }

    files.forEach((file) => {
      if (path.extname(file).toLowerCase() === '.svg') {
        const svgPath = path.join(svgDir, file);
        const pngPath = path.join(pngDir, `${path.basename(file, '.svg')}.png`);
        convertSVGtoPNG(svgPath, pngPath);
      }
    });
  });
}
main();
