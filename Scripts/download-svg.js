const axios = require('axios');
const fs = require('fs');
const puppeteer = require('puppeteer');
const path = require('path');

async function downloadSVG(url, filename) {
  try {
    const response = await axios.get(url, { responseType: 'arraybuffer' });

    const outputDir = './out/svg';
    const outputPath = path.join(outputDir, filename);

    await fs.promises.mkdir(outputDir, { recursive: true });

    fs.writeFileSync(outputPath, response.data);
    console.log(`Downloaded: ${filename}`);
  } catch (error) {
    console.error(`Error downloading ${url}: ${error.message}`);
  }
}

async function main() {
    const url = 'https://ge.globo.com/';

  const browser = await puppeteer.launch({
    headless: true,
    timeout: 0,
    args: ['--no-sandbox'],
  });

  const page = await browser.newPage();

  await page.setViewport({
    width: 1366,
    height: 768,
  });

  await page.goto(url, { timeout: 0 });

  const buttonTeam = await page.waitForSelector('#mosaico__wrapper');
  await buttonTeam.click();

  const links = await page.$$eval('img[src*=".svg"]', (imgs) => imgs.map((img) => img.src));

  for (const link of links) {
    const filename = link.split('/').pop();

    await downloadSVG(link, filename);
  }

  await browser.close();
}

main();
