import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { WeeklySchedule } from './components/WeeklySchedule';
import { Home } from "./components/Home";
import { ManageGroups } from './components/ManageGroups';
import { ManageSubjects } from './components/ManageSubjects';

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/groups-schedule',
    element: <WeeklySchedule key='groups' isGroup={true} />
  },
  {
    path: '/teachers-schedule',
    element: <WeeklySchedule key='teachers' isGroup={false} />
  },
  {
    path: '/manage-groups',
    requireAuth: true,
    element: <ManageGroups />
  },
  {
    path: '/manage-subjects',
    requireAuth: true,
    element: <ManageSubjects />
  },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
