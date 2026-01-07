const drop = document.getElementById('dropArea');
const tiles = document.getElementById('tiles');
const activity = document.getElementById('activity');
const fileInput = document.getElementById('fileInput');
const protocolInput = document.getElementById('protocolInput');
const openProtocolBtn = document.getElementById('openProtocol');
const clearBtn = document.getElementById('clearWorkspace');

function log(msg){
  const li = document.createElement('li'); li.textContent = msg; activity.prepend(li);
}

function addTile(obj){
  const el = document.createElement('div'); el.className='tile';
  const title = document.createElement('div'); title.className='title'; title.textContent = obj.name;
  const actions = document.createElement('div'); actions.className='actions';

  if(obj.preview){
    const preview = document.createElement('div'); preview.appendChild(obj.preview); el.appendChild(preview);
  }

  const openBtn = document.createElement('button'); openBtn.textContent='Open';
  openBtn.onclick = () => {
    if(obj.type === 'protocol'){
      // attempt to open protocol
      window.location.href = obj.url;
    } else if(obj.url){
      window.open(obj.url, '_blank');
    } else {
      alert('This file cannot be executed in a browser. You can download it.')
    }
  }

  const reveal = document.createElement('button'); reveal.textContent='Reveal';
  reveal.onclick = () => { alert('Files added to workspace only exist in your browser session. Use download to save.'); };

  actions.appendChild(openBtn); actions.appendChild(reveal);
  el.appendChild(title); el.appendChild(actions);
  tiles.prepend(el);
}

function handleFiles(list){
  for(const f of list){
    const name = f.name || f;
    const ext = name.split('.').pop().toLowerCase();

    if(ext === 'zip'){
      // extract with JSZip
      JSZip.loadAsync(f).then(zip => {
        log('Extracted zip: ' + name);
        zip.forEach((relativePath, zipEntry) => {
          if(!zipEntry.dir){
            zipEntry.async('blob').then(blob => {
              const url = URL.createObjectURL(blob);
              addTile({name: relativePath, url, type:'file'});
            });
          }
        });
      }).catch(err => log('Invalid zip: '+name));
      continue;
    }

    if(['png','jpg','jpeg','gif'].includes(ext)){
      const img = document.createElement('img'); img.style.maxWidth='100%'; img.style.maxHeight='60px';
      const url = URL.createObjectURL(f);
      img.src = url;
      addTile({name, preview:img, url, type:'image'});
      log('Added image: ' + name);
      continue;
    }

    if(['mp3','mp4','wav','ogg'].includes(ext)){
      const media = document.createElement(ext==='mp3' || ext==='wav' ? 'audio' : 'video'); media.controls=true; media.style.maxWidth='100%';
      const url = URL.createObjectURL(f); media.src = url;
      addTile({name, preview:media, url, type:'media'});
      log('Added media: ' + name);
      continue;
    }

    if(['txt','md','json','log'].includes(ext)){
      const reader = new FileReader();
      reader.onload = e => {
        const pre = document.createElement('pre'); pre.style.maxHeight='60px'; pre.style.overflow='hidden'; pre.textContent = e.target.result;
        addTile({name, preview:pre, type:'text'});
      };
      reader.readAsText(f);
      log('Added text: ' + name);
      continue;
    }

    if(ext === 'exe'){
      // cannot execute; show tile and allow download
      addTile({name, type:'exe'});
      log('Added .exe (cannot execute in browser): ' + name);
      continue;
    }

    // default: create blob URL and add
    const url = URL.createObjectURL(f);
    addTile({name, url, type:'file'});
    log('Added file: ' + name);
  }
}

drop.addEventListener('dragover', e=>{ e.preventDefault(); drop.classList.add('over'); });
drop.addEventListener('dragleave', e=>{ drop.classList.remove('over'); });
drop.addEventListener('drop', e=>{
  e.preventDefault(); drop.classList.remove('over');
  const dt = e.dataTransfer; if(!dt) return; const files = dt.files; if(files.length) handleFiles(files);
});

fileInput.addEventListener('change', e=>{ handleFiles(e.target.files); fileInput.value = null; });

openProtocolBtn.addEventListener('click', ()=>{
  const v = protocolInput.value.trim(); if(!v) return; try{ window.location.href = v; log('Attempted to open protocol: '+v); }catch(e){ log('Failed to open protocol: '+v); }
});

clearBtn.addEventListener('click', ()=>{ tiles.innerHTML=''; activity.innerHTML=''; log('Workspace cleared'); });

// If the page was opened with a hash like #protocol=discord://, try to parse and open
window.addEventListener('load', ()=>{
  const h = location.hash.substring(1);
  if(h.startsWith('protocol=')){
    const url = decodeURIComponent(h.split('=')[1]);
    try{ window.location.href = url; log('Opening protocol from hash: '+url); }catch{}
  }
});
