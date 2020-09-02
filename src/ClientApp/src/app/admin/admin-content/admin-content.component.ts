import { Component, OnInit } from '@angular/core';
import { VotingGuide, Option } from '../../services/data.service';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { select, Store } from '@ngrx/store';
import { ApplicationState } from '../../state/reducers';
import { getError, getGeneralInfo, getSelectedLanguage, getVotingGuide } from '../../state/selectors';
import { ClearErrorAction, UpdateDataAction } from '../../state/actions';
import { MatDialog } from '@angular/material/dialog';
import { VotersOptionEditorComponent } from './voters-option-editor/voters-option-editor.component';

@Component({
  selector: 'app-admin-content',
  templateUrl: './admin-content.component.html',
  styleUrls: ['./admin-content.component.scss']
})
export class AdminContentComponent implements OnInit {
  private selectedLanguage: string;

  public error: string;
  public generalInfo: string;
  public votersGuide: VotingGuide;
  public newVotersGuideOption: Option = {
    title: '',
    description: ''
  };
  public editing = {};
  public deleting = {};
  public editorConfig: AngularEditorConfig = {
    editable: true,
    toolbarHiddenButtons: [
      [
        'subscript',
        'superscript',
        'justifyLeft',
        'justifyCenter',
        'justifyRight',
        'justifyFull',
        'indent',
        'outdent',
        'heading',
        'fontName'
      ],
      [
        'textColor',
        'backgroundColor',
        'customClasses',
        'insertImage',
        'insertVideo',
        'insertHorizontalRule',
      ]
    ]
  };

  constructor(private store: Store<ApplicationState>,
    public dialog: MatDialog) {
  }

  public ngOnInit() {
    this.store.dispatch(new ClearErrorAction());

    this.store.pipe(select(getError)).subscribe(value => {
      this.error = value;
    });

    this.store.pipe(select(getSelectedLanguage)).subscribe(value => {
      this.selectedLanguage = value;
    });

    this.store.pipe(select(getGeneralInfo)).subscribe(value => {
      this.generalInfo = value;
    });

    this.store.pipe(select(getVotingGuide)).subscribe(value => {
      this.votersGuide = value || {
        description: '',
        options: []
      };
    });
  }

  public save() {
    this.store.dispatch(new ClearErrorAction());

    const data = {
      language: this.selectedLanguage,
      generalInfo: this.generalInfo,
      votersGuide: this.votersGuide,
    };

    this.store.dispatch(new UpdateDataAction(data, this.selectedLanguage));
  }

  public deleteVotersGuideOption(idx: number) {
    this.votersGuide.options.splice(idx, 1);
  }

  public openNewVotersGuideOption(): void {
    const dialogRef = this.dialog.open(VotersOptionEditorComponent);

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.title !== '' && result.description !== '') {
          this.votersGuide.options.push(result);
        }
      }
    });
  }

}
