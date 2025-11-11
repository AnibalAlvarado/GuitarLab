
import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
  AfterViewInit,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';
import {
  Factory,
  TabStave,
  Stave,
  TabNote,
  StaveNote,
  Voice,
  Formatter,
  StaveConnector,
} from 'vexflow';

@Component({
  selector: 'app-tab-editor',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tab-editor.component.html',
  styleUrls: ['./tab-editor.component.css'],
})
export class TabEditorComponent implements OnInit, OnChanges, AfterViewInit {
  @Input() tabData = '';
  @Output() tabDataChange = new EventEmitter<string>();

  @ViewChild('vexContainer', { static: true })
  vexContainer!: ElementRef<HTMLDivElement>;

  private factory!: Factory;
  private context!: any;
  private initialized = false;

  strings = ['E', 'B', 'G', 'D', 'A', 'E'];
  grid: string[][] = [];
  initialColumns = 16;

  ngOnInit() {
    if (this.tabData?.trim()) this.parseTabData(this.tabData);
    else this.grid = this.strings.map(() => Array(this.initialColumns).fill('-'));
  }

  ngAfterViewInit() {
    this.initVexRenderer();
    this.updateTab();
    this.initialized = true;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.initialized && changes['tabData'] && !changes['tabData'].firstChange) {
      this.parseTabData(this.tabData);
      this.renderTab();
    }
  }

  /** 🔹 Captura la edición del usuario en una celda */
  onNoteChange(event: Event, row: number, col: number) {
    const value = (event.target as HTMLInputElement).value.trim();
    this.grid[row][col] = value === '' ? '-' : value;
    this.updateTab();
  }

  /** 🔹 trackBy para mejorar el rendimiento de ngFor */
  trackByIndex(index: number): number {
    return index;
  }

  initializeEmptyGrid(columns: number) {
    this.grid = this.strings.map(() => Array(columns).fill('-'));
    this.updateTab();
  }

  parseTabData(data: string) {
    if (!data?.trim()) {
      this.initializeEmptyGrid(this.initialColumns);
      return;
    }

    const lines = data.trim().split('\n').filter((l) => l.trim());
    if (lines.length !== this.strings.length) {
      this.initializeEmptyGrid(this.initialColumns);
      return;
    }

    const parsed: string[][] = [];
    let maxLen = 0;

    for (const line of lines) {
      const match = line.match(/\|(.+)\|/);
      const content = match ? match[1] : '';
      const notes: string[] = [];

      for (let i = 0; i < content.length; i++) {
        const ch = content[i];
        const next = content[i + 1];
        if (/\d/.test(ch) && /\d/.test(next)) {
          notes.push(ch + next);
          i++;
        } else notes.push(ch);
      }

      parsed.push(notes);
      maxLen = Math.max(maxLen, notes.length);
    }

    for (const row of parsed) while (row.length < maxLen) row.push('-');
    this.grid = parsed;
  }

  updateTab() {
    this.tabData = this.grid.map((r, i) => `${this.strings[i]}|${r.join('')}|`).join('\n');
    if (this.initialized) this.tabDataChange.emit(this.tabData);
    this.renderTab();
  }

  addColumns(count: number) {
    this.grid.forEach((r) => r.push(...Array(count).fill('-')));
    this.updateTab();
  }

  removeLastColumns(count: number) {
    const newLen = Math.max(4, this.grid[0].length - count);
    this.grid.forEach((r) => r.splice(newLen));
    this.updateTab();
  }

  clearAll() {
    Swal.fire({
      title: 'Clear all?',
      text: 'Are you sure you want to clear the entire tablature?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Yes, clear it',
      cancelButtonText: 'Cancel',
      background: '#1a0b2e',
      color: '#fff',
    }).then((r) => {
      if (r.isConfirmed) {
        this.initializeEmptyGrid(this.initialColumns);
        Swal.fire({
          icon: 'success',
          title: 'Cleared!',
          text: 'The tablature grid has been reset.',
          timer: 1500,
          showConfirmButton: false,
          background: '#1a0b2e',
          color: '#fff',
        });
      }
    });
  }

  getTotalNotes(): number {
    return this.grid.reduce(
      (sum, row) => sum + row.filter((n) => n !== '-' && n.trim() !== '').length,
      0
    );
  }

  /** 🎼 Inicializa el renderer SVG de VexFlow */
  private initVexRenderer() {
    this.factory = new Factory({
      renderer: {
        elementId: this.vexContainer.nativeElement.id,
        width: 900,
        height: 320,
      },
    });
    this.context = this.factory.getContext();
  }

  /** 🎸 Render combinado: pentagrama + tablatura */
  private renderTab() {
    if (!this.factory) return;

    const ctx = this.context;
    ctx.clear();

    const staveWidth = 850;
    const staveY = 40;

    const staveNotation = new Stave(10, staveY, staveWidth);
    const staveTab = new TabStave(10, staveY + 100, staveWidth);

    staveNotation.addClef('treble');
    staveTab.addTabGlyph();

    staveNotation.setContext(ctx).draw();
    staveTab.setContext(ctx).draw();

    const tabNotes: TabNote[] = [];
    const staveNotes: StaveNote[] = [];

    for (let i = 0; i < this.grid[0].length; i++) {
      const positions: any[] = [];
      for (let s = 0; s < this.strings.length; s++) {
        const val = this.grid[this.strings.length - 1 - s][i];
        if (val !== '-' && val.trim() !== '') {
          const fret = parseInt(val);
          if (!isNaN(fret)) positions.push({ str: s + 1, fret });
        }
      }

      if (positions.length > 0) {
        tabNotes.push(new TabNote({ positions, duration: 'q' }));
        staveNotes.push(new StaveNote({ keys: ['b/4'], duration: 'q' })); // dummy pitch
      }
    }

    if (tabNotes.length === 0) return;

    const voice = new Voice({
      numBeats: tabNotes.length,
      beatValue: 4,
    });

    voice.addTickables(tabNotes);
    new Formatter().joinVoices([voice]).format([voice], staveWidth - 50);
    voice.draw(ctx, staveTab);

    const connector = new StaveConnector(staveNotation, staveTab);
    connector.setType(3); // doble línea vertical
    connector.setContext(ctx).draw();
  }

  
}
