import React, { Component } from 'react';
import { Collapse, DropdownItem, DropdownMenu, DropdownToggle, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, UncontrolledDropdown } from 'reactstrap';
import { Link } from 'react-router-dom';
import { LoginMenu } from './api-authorization/LoginMenu';
import './NavMenu.css';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar() {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render() {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
          <NavbarBrand tag={Link} to="/">E-Journal</NavbarBrand>
          <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
          <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
            <ul className="navbar-nav flex-grow">
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/">Главная</NavLink>
              </NavItem>

              <UncontrolledDropdown inNavbar nav>
                <DropdownToggle caret nav>Администрирование</DropdownToggle>
                <DropdownMenu right>

                  <DropdownItem>
                    <NavLink tag={Link} className="text-dark" to="/manage-trainings">Занятия</NavLink>
                  </DropdownItem>

                  <DropdownItem>
                    <NavLink tag={Link} className="text-dark" to="/manage-students">Учащиеся</NavLink>
                  </DropdownItem>

                  <DropdownItem divider />

                  <DropdownItem>
                    <NavLink tag={Link} className="text-dark" to="/manage-groups">Группы</NavLink>
                  </DropdownItem>

                  <DropdownItem>
                    <NavLink tag={Link} className="text-dark" to="/manage-subjects">Дисциплины</NavLink>
                  </DropdownItem>

                  <DropdownItem>
                    <NavLink tag={Link} className="text-dark" to="/manage-teachers">Преподаватели</NavLink>
                  </DropdownItem>

                  <DropdownItem>
                    <NavLink tag={Link} className="text-dark" to="/manage-training-types">Типы занятий</NavLink>
                  </DropdownItem>

                  <DropdownItem>
                    <NavLink tag={Link} className="text-dark" to="/manage-mark-values">Значения оценок</NavLink>
                  </DropdownItem>

                </DropdownMenu>
              </UncontrolledDropdown>

              <UncontrolledDropdown inNavbar nav>
                <DropdownToggle caret nav>Расписания</DropdownToggle>
                <DropdownMenu>

                  <DropdownItem>
                    <NavLink tag={Link} className="text-dark" to="/groups-schedule">Расписание групп</NavLink>
                  </DropdownItem>

                  <DropdownItem>
                    <NavLink tag={Link} className="text-dark" to="/teachers-schedule">Расписание преподавателей</NavLink>
                  </DropdownItem>

                </DropdownMenu>
              </UncontrolledDropdown>

              <LoginMenu>
              </LoginMenu>
            </ul>
          </Collapse>
        </Navbar>
      </header>
    );
  }
}
