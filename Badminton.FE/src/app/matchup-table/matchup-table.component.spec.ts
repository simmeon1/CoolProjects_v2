import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MatchupTable } from './matchup-table.component';

describe('MatchupTable', () => {
  let component: MatchupTable;
  let fixture: ComponentFixture<MatchupTable>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatchupTable]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MatchupTable);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
