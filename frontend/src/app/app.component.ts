import {Component, OnInit} from '@angular/core';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
})
export class AppComponent implements OnInit {
  isMobile: boolean | undefined;
  allRooms: number[] = [1, 2, 3]; //allRooms: Room[] | undefined;
  allDevices: number[] = [1, 2, 3]; //allDevices: Device[] | undefined;
  menuItems: MenuItem[] = [
    {
      label: 'Auth eksempel',
      icon: 'person-circle',
      subItems: [
        {label: 'Login', routerLink: 'login'},
        {label: 'Register', routerLink: 'register'}
      ]
    }
  ];

  constructor(private breakpointObserver: BreakpointObserver) {
    this.breakpointObserver.observe([
      Breakpoints.HandsetPortrait
    ]).subscribe(result => {
      this.isMobile = result.matches;
    });
  }

  async ngOnInit(): Promise<void> {
    await this.loadRooms();
    await this.loadDevices();
    await this.setSubItemIcon();
  }

  loadRooms() {
    //TODO: Load logged in user's rooms (max amount?)
    var item: MenuItem = {
      label: 'Rooms',
      icon: 'grid',
      subItems: []
    }

    //TODO: Load logged in user's rooms (max amount?)
    for (var r of this.allRooms) {
      this.addSubItem('Room '+r.toString(), 'rooms/'+r.toString(), item)
    }

    this.addSubItem('All rooms', 'rooms/all', item)
    this.menuItems.push(item)
  }

  loadDevices() {
    //TODO: Load logged in user's devices (max amount?)
    var item: MenuItem = {
      label: 'Devices',
      icon: 'fitness',
      subItems: []
    }

    //TODO: Load logged in user's device (max amount?)
    for (var d of this.allDevices) {
      this.addSubItem('Device '+d, 'devices/'+d, item)
    }

    this.menuItems.push(item)
  }

  pushToMenuItems(item: MenuItem[]) {
    item.forEach(i => this.menuItems.push(i))
  }

  addSubItem(label: string, link: string, menuItem: MenuItem){
    var subItem: SubItem = {
      label: label, routerLink: link
    }
    menuItem.subItems?.push(subItem);
  }

  setSubItemIcon() {
    this.menuItems.forEach(menuItem => {
      if (menuItem.subItems) {
        menuItem.subItems.forEach(subItem => {
          subItem.icon = 'chevron-forward';
        });
      }
    });
  }
}

interface MenuItem {
  label: string;
  icon?: string;
  subItems?: SubItem[];
}

interface SubItem extends MenuItem {
  routerLink?: string;
}
