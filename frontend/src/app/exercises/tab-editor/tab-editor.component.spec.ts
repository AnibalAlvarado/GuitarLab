import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TabEditorComponent } from './tab-editor.component';

describe('TabEditorComponent', () => {
  let component: TabEditorComponent;
  let fixture: ComponentFixture<TabEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TabEditorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TabEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
