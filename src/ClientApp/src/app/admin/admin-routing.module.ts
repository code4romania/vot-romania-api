import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { AdminComponent } from './admin.component';
import { AuthGuard } from './services/auth.guard';
import { AdminContentComponent } from './admin-content/admin-content.component';
import { ImportPollingStationsComponent } from './import-polling-stations/import-polling-stations.component';

const routes: Routes = [
  { path: 'admin', component: AdminComponent, canActivate: [AuthGuard] },
  { path: 'admin/content', component: AdminContentComponent, canActivate: [AuthGuard] },
  { path: 'admin/import', component: ImportPollingStationsComponent, canActivate: [AuthGuard] },

  { path: 'admin/login', component: LoginComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
