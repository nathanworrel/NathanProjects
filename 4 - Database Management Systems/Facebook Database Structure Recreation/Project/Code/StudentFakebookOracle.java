package project2;

import java.sql.Connection;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.ResultSet;
import java.util.ArrayList;

/*
    The StudentFakebookOracle class is derived from the FakebookOracle class and implements
    the abstract query functions that investigate the database provided via the <connection>
    parameter of the constructor to discover specific information.
*/
public final class StudentFakebookOracle extends FakebookOracle {
    // [Constructor]
    // REQUIRES: <connection> is a valid JDBC connection
    public StudentFakebookOracle(Connection connection) {
        oracle = connection;
    }

    @Override
    // Query 0
    // -----------------------------------------------------------------------------------
    // GOALS: (A) Find the total number of users for which a birth month is listed
    //        (B) Find the birth month in which the most users were born
    //        (C) Find the birth month in which the fewest users (at least one) were born
    //        (D) Find the IDs, first names, and last names of users born in the month
    //            identified in (B)
    //        (E) Find the IDs, first names, and last name of users born in the month
    //            identified in (C)
    //
    // This query is provided to you completed for reference. Below you will find the appropriate
    // mechanisms for opening up a statement, executing a query, walking through results, extracting
    // data, and more things that you will need to do for the remaining nine queries
    public BirthMonthInfo findMonthOfBirthInfo() throws SQLException {
        try (Statement stmt = oracle.createStatement(FakebookOracleConstants.AllScroll,
                FakebookOracleConstants.ReadOnly)) {
            // Step 1
            // ------------
            // * Find the total number of users with birth month info
            // * Find the month in which the most users were born
            // * Find the month in which the fewest (but at least 1) users were born
            ResultSet rst = stmt.executeQuery(
                    "SELECT COUNT(*) AS Birthed, Month_of_Birth " + // select birth months and number of uses with that birth month
                            "FROM " + UsersTable + " " + // from all users
                            "WHERE Month_of_Birth IS NOT NULL " + // for which a birth month is available
                            "GROUP BY Month_of_Birth " + // group into buckets by birth month
                            "ORDER BY Birthed DESC, Month_of_Birth ASC"); // sort by users born in that month, descending; break ties by birth month

            int mostMonth = 0;
            int leastMonth = 0;
            int total = 0;
            while (rst.next()) { // step through result rows/records one by one
                if (rst.isFirst()) { // if first record
                    mostMonth = rst.getInt(2); //   it is the month with the most
                }
                if (rst.isLast()) { // if last record
                    leastMonth = rst.getInt(2); //   it is the month with the least
                }
                total += rst.getInt(1); // get the first field's value as an integer
            }
            BirthMonthInfo info = new BirthMonthInfo(total, mostMonth, leastMonth);

            // Step 2
            // ------------
            // * Get the names of users born in the most popular birth month
            rst = stmt.executeQuery(
                    "SELECT User_ID, First_Name, Last_Name " + // select ID, first name, and last name
                            "FROM " + UsersTable + " " + // from all users
                            "WHERE Month_of_Birth = " + mostMonth + " " + // born in the most popular birth month
                            "ORDER BY User_ID"); // sort smaller IDs first

            while (rst.next()) {
                info.addMostPopularBirthMonthUser(new UserInfo(rst.getLong(1), rst.getString(2), rst.getString(3)));
            }

            // Step 3
            // ------------
            // * Get the names of users born in the least popular birth month
            rst = stmt.executeQuery(
                    "SELECT User_ID, First_Name, Last_Name " + // select ID, first name, and last name
                            "FROM " + UsersTable + " " + // from all users
                            "WHERE Month_of_Birth = " + leastMonth + " " + // born in the least popular birth month
                            "ORDER BY User_ID"); // sort smaller IDs first

            while (rst.next()) {
                info.addLeastPopularBirthMonthUser(new UserInfo(rst.getLong(1), rst.getString(2), rst.getString(3)));
            }

            // Step 4
            // ------------
            // * Close resources being used
            rst.close();
            stmt.close(); // if you close the statement first, the result set gets closed automatically

            return info;

        } catch (SQLException e) {
            System.err.println(e.getMessage());
            return new BirthMonthInfo(-1, -1, -1);
        }
    }

    @Override
    // Query 1
    // -----------------------------------------------------------------------------------
    // GOALS: (A) The first name(s) with the most letters
    //        (B) The first name(s) with the fewest letters
    //        (C) The first name held by the most users
    //        (D) The number of users whose first name is that identified in (C)
    public FirstNameInfo findNameInfo() throws SQLException {
        try (Statement stmt = oracle.createStatement(FakebookOracleConstants.AllScroll,
                FakebookOracleConstants.ReadOnly)) {
            FirstNameInfo info = new FirstNameInfo();
            ResultSet rst = stmt.executeQuery(
                    "SELECT DISTINCT LENGTH(First_Name) AS Length, First_Name " + 
                            "FROM " + UsersTable + " " + 
                            "ORDER BY Length DESC, First_Name ASC");
            
            int longest = 0;
            int shortest = 0;
            ArrayList<String> hold = new ArrayList<String>();
            while (rst.next()) {
                if (rst.isFirst()) { // if first record
                    longest = rst.getInt(1);
                    shortest = longest;
                    info.addLongName(rst.getString(2));
                }
                else if(rst.getInt(1) == longest){
                    info.addLongName(rst.getString(2));
                }
                if(rst.getInt(1) <= shortest){
                    if(rst.getInt(1) < shortest){
                        hold.clear();
                        shortest = rst.getInt(1);
                    }
                    hold.add(rst.getString(2));
                }
            }
            for (String string : hold) {
                info.addShortName(string);
            }

            rst = stmt.executeQuery(
                    "SELECT DISTINCT COUNT(*) AS Count_Name, First_Name " + 
                            "FROM " + UsersTable + " " + 
                            "GROUP BY First_Name " +
                            "ORDER BY Count_Name DESC, First_Name ASC");
            
            while (rst.next()) {
                if (rst.isFirst()) { // if first record
                    longest = rst.getInt(1);
                    info.addCommonName(rst.getString(2));
                }
                else if(rst.getInt(1) == longest){
                    info.addCommonName(rst.getString(2));
                }else{
                    break;
                }
            }

            info.setCommonNameCount(longest);

            return info;
            //return new FirstNameInfo(); // placeholder for compilation
        } catch (SQLException e) {
            System.err.println(e.getMessage());
            return new FirstNameInfo();
        }
    }

    @Override
    // Query 2
    // -----------------------------------------------------------------------------------
    // GOALS: (A) Find the IDs, first names, and last names of users without any friends
    //
    // Be careful! Remember that if two users are friends, the Friends table only contains
    // the one entry (U1, U2) where U1 < U2.
    public FakebookArrayList<UserInfo> lonelyUsers() throws SQLException {
        FakebookArrayList<UserInfo> results = new FakebookArrayList<UserInfo>(", ");

        try (Statement stmt = oracle.createStatement(FakebookOracleConstants.AllScroll,
                FakebookOracleConstants.ReadOnly)) {
            /*
                EXAMPLE DATA STRUCTURE USAGE
                ============================================
                UserInfo u1 = new UserInfo(15, "Abraham", "Lincoln");
                UserInfo u2 = new UserInfo(39, "Margaret", "Thatcher");
                results.add(u1);
                results.add(u2);
            */
            ResultSet rst = stmt.executeQuery(
                    "SELECT DISTINCT U.USER_ID, U.First_Name, U.Last_Name " + 
                            "FROM " + UsersTable + " U " +
                            "WHERE U.USER_ID NOT IN (SELECT USER1_ID FROM "+ FriendsTable + " UNION SELECT USER2_ID FROM "+ FriendsTable + " )" +
                            "ORDER BY U.USER_ID ASC");
            
            while (rst.next()) {
                UserInfo u = new UserInfo(rst.getInt(1), rst.getString(2), rst.getString(3));
                results.add(u);
            }
        } catch (SQLException e) {
            System.err.println(e.getMessage());
        }

        return results;
    }

    @Override
    // Query 3
    // -----------------------------------------------------------------------------------
    // GOALS: (A) Find the IDs, first names, and last names of users who no longer live
    //            in their hometown (i.e. their current city and their hometown are different)
    public FakebookArrayList<UserInfo> liveAwayFromHome() throws SQLException {
        FakebookArrayList<UserInfo> results = new FakebookArrayList<UserInfo>(", ");

        try (Statement stmt = oracle.createStatement(FakebookOracleConstants.AllScroll,
                FakebookOracleConstants.ReadOnly)) {
            ResultSet rst = stmt.executeQuery(
                    "SELECT DISTINCT U.USER_ID, U.First_Name, U.Last_Name " + 
                            "FROM " + UsersTable + " U, " + CurrentCitiesTable + " C, " + HometownCitiesTable + " H " +
                            "WHERE U.USER_ID = C.USER_ID AND U.USER_ID = H.USER_ID AND C.CURRENT_CITY_ID != H.HOMETOWN_CITY_ID " +
                            "ORDER BY USER_ID ASC");
            
            while (rst.next()) {
                UserInfo u = new UserInfo(rst.getInt(1), rst.getString(2), rst.getString(3));
                results.add(u);
            }
        } catch (SQLException e) {
            System.err.println(e.getMessage());
        }

        return results;
    }

    @Override
    // Query 4
    // -----------------------------------------------------------------------------------
    // GOALS: (A) Find the IDs, links, and IDs and names of the containing album of the top
    //            <num> photos with the most tagged users
    //        (B) For each photo identified in (A), find the IDs, first names, and last names
    //            of the users therein tagged
    public FakebookArrayList<TaggedPhotoInfo> findPhotosWithMostTags(int num) throws SQLException {
        FakebookArrayList<TaggedPhotoInfo> results = new FakebookArrayList<TaggedPhotoInfo>("\n");

        try (Statement stmt = oracle.createStatement(FakebookOracleConstants.AllScroll,
                FakebookOracleConstants.ReadOnly)) {
            stmt.executeUpdate(
                "CREATE OR REPLACE VIEW TAG_IDS_HOLD AS " +
                "SELECT * FROM ( " +
                "SELECT DISTINCT COUNT(*) AS COUNT_TAG, TAG_PHOTO_ID " + 
                "FROM " + TagsTable + " " + 
                "GROUP BY TAG_PHOTO_ID " +
                "ORDER BY Count_TAG DESC, TAG_PHOTO_ID ASC" + " ) WHERE ROWNUM <= "+ num);
            ResultSet rs = stmt.executeQuery(
                "SELECT DISTINCT U.USER_ID, U.FIRST_NAME, U.LAST_NAME, I.TAG_PHOTO_ID, P.ALBUM_ID, P.PHOTO_LINK, A.ALBUM_NAME, I.COUNT_TAG " +
                "FROM " + TagsTable + " T, " + UsersTable + " U, " + PhotosTable + " P, " + AlbumsTable + " A, TAG_IDS_HOLD I " +
                "WHERE T.TAG_PHOTO_ID = I.TAG_PHOTO_ID AND T.TAG_SUBJECT_ID = U.USER_ID AND T.TAG_PHOTO_ID = P.PHOTO_ID AND P.ALBUM_ID = A.ALBUM_ID " +
                "ORDER BY I.COUNT_TAG DESC, TAG_PHOTO_ID ASC, U.USER_ID ASC");
            int i = 0;
            int id = -1;
            TaggedPhotoInfo tp = null;
            while(rs.next() && i<num){
                if(id != rs.getInt(4)){
                    if(id != -1){
                        results.add(tp);
                        i++;
                    }
                    id = rs.getInt(4);
                    PhotoInfo p = new PhotoInfo(rs.getInt(4), rs.getInt(5), rs.getString(6), rs.getString(7));
                    tp = new TaggedPhotoInfo(p);
                }
                tp.addTaggedUser(new UserInfo(rs.getInt(1), rs.getString(2), rs.getString(3)));
            }
            if(i<num && id != -1){
                results.add(tp);
            }    

            stmt.executeUpdate("DROP VIEW TAG_IDS_HOLD");

        } catch (SQLException e) {
            System.err.println(e.getMessage());
        }

        return results;
    }

    @Override
    // Query 5
    // -----------------------------------------------------------------------------------
    // GOALS: (A) Find the IDs, first names, last names, and birth years of each of the two
    //            users in the top <num> pairs of users that meet each of the following
    //            criteria:
    //              (i) same gender
    //              (ii) tagged in at least one common photo
    //              (iii) difference in birth years is no more than <yearDiff>
    //              (iv) not friends
    //        (B) For each pair identified in (A), find the IDs, links, and IDs and names of
    //            the containing album of each photo in which they are tagged together
    public FakebookArrayList<MatchPair> matchMaker(int num, int yearDiff) throws SQLException {
        FakebookArrayList<MatchPair> results = new FakebookArrayList<MatchPair>("\n");

        try (Statement stmt = oracle.createStatement(FakebookOracleConstants.AllScroll,
                FakebookOracleConstants.ReadOnly)) {
            stmt.executeUpdate(
                "CREATE OR REPLACE VIEW USERS_HOLD AS " +
                "SELECT DISTINCT U.USER_ID AS US1, H.USER_ID AS US2, COUNT(*) AS COUNTER " + 
                "FROM " + UsersTable + " U, " + UsersTable + " H, " + TagsTable + " T, " + 
                TagsTable + " G " +
                "WHERE U.USER_ID < H.USER_ID AND U.GENDER = H.GENDER AND ABS(U.YEAR_OF_BIRTH - H.YEAR_OF_BIRTH) <= "+ yearDiff + " " +
                "AND NOT EXISTS ( SELECT F.USER1_ID, F.USER2_ID FROM " + 
                FriendsTable + " F WHERE F.USER1_ID = U.USER_ID AND F.USER2_ID = H.USER_ID ) " + 
                "AND T.TAG_SUBJECT_ID = U.USER_ID AND G.TAG_SUBJECT_ID = H.USER_ID AND T.TAG_PHOTO_ID = G.TAG_PHOTO_ID " +
                "GROUP BY U.USER_ID, H.USER_ID");
            ResultSet rst = stmt.executeQuery(
                "SELECT DISTINCT UF.USER_ID, UF.FIRST_NAME, UF.LAST_NAME, UF.YEAR_OF_BIRTH, " +
                "US.USER_ID, US.FIRST_NAME, US.LAST_NAME, US.YEAR_OF_BIRTH, " +
                "TF.TAG_PHOTO_ID, A.ALBUM_ID, P.PHOTO_LINK, A.ALBUM_NAME, U.COUNTER " +
                "FROM USERS_HOLD U, " + UsersTable + " UF, " + UsersTable + " US, " + 
                TagsTable + " TF, "  + TagsTable + " TS, " + PhotosTable + " P, " + AlbumsTable + " A " + 
                "WHERE U.US1 = UF.USER_ID AND U.US2 = US.USER_ID AND " +
                "TF.TAG_SUBJECT_ID = U.US1 AND TS.TAG_SUBJECT_ID = U.US2 AND TF.TAG_PHOTO_ID = TS.TAG_PHOTO_ID " +
                "AND TF.TAG_PHOTO_ID = P.PHOTO_ID AND P.ALBUM_ID = A.ALBUM_ID " +
                "ORDER BY U.COUNTER DESC, UF.USER_ID ASC, US.USER_ID ASC, TF.TAG_PHOTO_ID ASC");
            
            int u1id = -1;
            int u2id = -1;
            MatchPair mp = null;
            int i = 0;
            while(rst.next() && i < num){
                if(u1id != rst.getInt(1) || u2id != rst.getInt(5)){
                    if(u1id != -1){
                        results.add(mp);
                        i++;
                    }
                    u1id = rst.getInt(1);
                    u2id = rst.getInt(5);
                    UserInfo u1 = new UserInfo(u1id, rst.getString(2), rst.getString(3));
                    UserInfo u2 = new UserInfo(u2id, rst.getString(6), rst.getString(7));
                    mp = new MatchPair(u1, rst.getInt(4), u2,  rst.getInt(8));
                }
                mp.addSharedPhoto(new PhotoInfo(rst.getInt(9), rst.getInt(10), rst.getString(11), rst.getString(12)));

            }
            if(u1id != -1 && i < num){
                results.add(mp);
            }

            stmt.executeUpdate("DROP VIEW USERS_HOLD");

        } catch (SQLException e) {
            System.err.println(e.getMessage());
        }

        return results;
    }

    @Override
    // Query 6
    // -----------------------------------------------------------------------------------
    // GOALS: (A) Find the IDs, first names, and last names of each of the two users in
    //            the top <num> pairs of users who are not friends but have a lot of
    //            common friends
    //        (B) For each pair identified in (A), find the IDs, first names, and last names
    //            of all the two users' common friends
    public FakebookArrayList<UsersPair> suggestFriends(int num) throws SQLException {
        FakebookArrayList<UsersPair> results = new FakebookArrayList<UsersPair>("\n");

        try (Statement stmt = oracle.createStatement(FakebookOracleConstants.AllScroll,
                FakebookOracleConstants.ReadOnly)) {
            /*
                EXAMPLE DATA STRUCTURE USAGE
                ============================================
                UserInfo u1 = new UserInfo(16, "The", "Hacker");
                UserInfo u2 = new UserInfo(80, "Dr.", "Marbles");
                UserInfo u3 = new UserInfo(192, "Digit", "Le Boid");
                UsersPair up = new UsersPair(u1, u2);
                up.addSharedFriend(u3);
                results.add(up);
            */
            stmt.executeUpdate(
                "CREATE OR REPLACE VIEW HOLD_ALL_F AS " +
                "SELECT DISTINCT F.USER1_ID AS USER1, F.USER2_ID AS USER2 " + 
                "FROM " + FriendsTable + " F " +
                "UNION " +
                "SELECT DISTINCT G.USER2_ID AS USER1, G.USER1_ID AS USER2 " + 
                "FROM " + FriendsTable + " G");
            stmt.executeUpdate(
                "CREATE OR REPLACE VIEW NO_POSSIBLE_RESTRICTION AS " +
                "SELECT DISTINCT U.USER_ID AS US1, H.USER_ID AS US2, COUNT(*) AS COUNTER " + 
                "FROM " + UsersTable + " U, " + UsersTable + " H, HOLD_ALL_F FU, HOLD_ALL_F FH "+
                "WHERE U.USER_ID < H.USER_ID " +
                "AND NOT EXISTS ( SELECT F.USER1_ID, F.USER2_ID FROM " + 
                FriendsTable + " F WHERE F.USER1_ID = U.USER_ID AND F.USER2_ID = H.USER_ID ) " + 
                "AND FU.USER1 = U.USER_ID AND FH.USER1 = H.USER_ID AND FU.USER2 = FH.USER2 " +
                "GROUP BY U.USER_ID, H.USER_ID " +
                "ORDER BY COUNTER DESC, US1 ASC, US2 ASC " +  "OFFSET 0 ROWS FETCH NEXT "+num+" ROWS ONLY");

                ResultSet rst = stmt.executeQuery(
                    "SELECT DISTINCT UF.USER_ID, UF.FIRST_NAME, UF.LAST_NAME, " +
                    "US.USER_ID, US.FIRST_NAME, US.LAST_NAME, " +
                    "UT.USER_ID, UT.FIRST_NAME, UT.LAST_NAME, U.COUNTER " +
                    "FROM NO_POSSIBLE_RESTRICTION U, " + UsersTable + " UF, " + UsersTable + " US, " + 
                    UsersTable + " UT, HOLD_ALL_F FU, HOLD_ALL_F FH " +
                    "WHERE U.US1 = UF.USER_ID AND U.US2 = US.USER_ID " +
                    "AND FU.USER1 = UF.USER_ID AND FH.USER1 = US.USER_ID AND FU.USER2 = FH.USER2 " +
                    "AND UT.USER_ID = FU.USER2 " +
                    "ORDER BY U.COUNTER DESC, UF.USER_ID ASC, US.USER_ID ASC, UT.USER_ID ASC");
                
                int u1id = -1;
                int u2id = -1;
                UsersPair up = null;
                int i = 0;
                while(rst.next() && i < num){
                    if(u1id != rst.getInt(1) || u2id != rst.getInt(4)){
                        if(u1id != -1){
                            results.add(up);
                            i++;
                        }
                        u1id = rst.getInt(1);
                        u2id = rst.getInt(4);
                        UserInfo u1 = new UserInfo(u1id, rst.getString(2), rst.getString(3));
                        UserInfo u2 = new UserInfo(u2id, rst.getString(5), rst.getString(6));
                        up = new UsersPair(u1, u2);
                    }
                    up.addSharedFriend(new UserInfo(rst.getInt(7), rst.getString(8), rst.getString(9)));
                }
                if(u1id != -1 && i < num){
                    results.add(up);
                }

            stmt.executeUpdate("DROP VIEW NO_POSSIBLE_RESTRICTION");
            stmt.executeUpdate("DROP VIEW HOLD_ALL_F");
                
        } catch (SQLException e) {
            System.err.println(e.getMessage());
        }

        return results;
    }

    @Override
    // Query 7
    // -----------------------------------------------------------------------------------
    // GOALS: (A) Find the name of the state or states in which the most events are held
    //        (B) Find the number of events held in the states identified in (A)
    public EventStateInfo findEventStates() throws SQLException {
        try (Statement stmt = oracle.createStatement(FakebookOracleConstants.AllScroll,
                FakebookOracleConstants.ReadOnly)) {
            ResultSet rst = stmt.executeQuery(
                "SELECT DISTINCT C.STATE_NAME, COUNT(*) AS COUNTER " + 
                "FROM " + EventsTable + " E, " + CitiesTable + " C " + 
                "WHERE E.EVENT_CITY_ID = C.CITY_ID " +
                "GROUP BY C.STATE_NAME " +
                "ORDER BY COUNTER DESC, C.STATE_NAME ASC");
            
            int top = -1;
            EventStateInfo info = null;
            while(rst.next()){
                if(top == -1){
                    top = rst.getInt(2);
                    info = new EventStateInfo(top);
                }
                if(rst.getInt(2) == top){
                    info.addState(rst.getString(1));
                }else{
                    break;
                }
            }

            return(info);
        } catch (SQLException e) {
            System.err.println(e.getMessage());
            return new EventStateInfo(-1);
        }
    }

    @Override
    // Query 8
    // -----------------------------------------------------------------------------------
    // GOALS: (A) Find the ID, first name, and last name of the oldest friend of the user
    //            with User ID <userID>
    //        (B) Find the ID, first name, and last name of the youngest friend of the user
    //            with User ID <userID>
    public AgeInfo findAgeInfo(long userID) throws SQLException {
        try (Statement stmt = oracle.createStatement(FakebookOracleConstants.AllScroll,
                FakebookOracleConstants.ReadOnly)) {
            UserInfo old = new UserInfo(12000000, "Galileo", "Galilei");
            UserInfo young = new UserInfo(80000000, "Neil", "deGrasse Tyson");
            ResultSet rst = stmt.executeQuery(
                "SELECT DISTINCT U.USER_ID, U.FIRST_NAME, U.LAST_NAME, U.YEAR_OF_BIRTH, U.MONTH_OF_BIRTH, U.DAY_OF_BIRTH " + 
                "FROM " + FriendsTable + " F, " + UsersTable + " U " + 
                "WHERE ( ( F.USER1_ID = " + userID + " AND F.USER2_ID = U.USER_ID ) OR ( F.USER2_ID = " + userID + " AND F.USER1_ID = U.USER_ID ) ) " +
                "ORDER BY U.YEAR_OF_BIRTH ASC, U.MONTH_OF_BIRTH ASC, U.DAY_OF_BIRTH ASC, U.USER_ID DESC " +
                "OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY");
            while(rst.next()){
                if(rst.isFirst()){
                    old = new UserInfo(rst.getInt(1), rst.getString(2), rst.getString(3));
                }
            }
            rst = stmt.executeQuery(
                "SELECT DISTINCT U.USER_ID, U.FIRST_NAME, U.LAST_NAME, U.YEAR_OF_BIRTH, U.MONTH_OF_BIRTH, U.DAY_OF_BIRTH " + 
                "FROM " + FriendsTable + " F, " + UsersTable + " U " + 
                "WHERE ( ( F.USER1_ID = " + userID + " AND F.USER2_ID = U.USER_ID ) OR ( F.USER2_ID = " + userID + " AND F.USER1_ID = U.USER_ID ) ) " +
                "ORDER BY U.YEAR_OF_BIRTH DESC, U.MONTH_OF_BIRTH DESC, U.DAY_OF_BIRTH DESC, U.USER_ID DESC " +
                "OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY");
            while(rst.next()){
                if(rst.isFirst()){
                    young = new UserInfo(rst.getInt(1), rst.getString(2), rst.getString(3));
                }
            }
            return new AgeInfo(old, young);
        } catch (SQLException e) {
            System.err.println(e.getMessage());
            return new AgeInfo(new UserInfo(-1, "ERROR", "ERROR"), new UserInfo(-1, "ERROR", "ERROR"));
        }
    }

    @Override
    // Query 9
    // -----------------------------------------------------------------------------------
    // GOALS: (A) Find all pairs of users that meet each of the following criteria
    //              (i) same last name
    //              (ii) same hometown
    //              (iii) are friends
    //              (iv) less than 10 birth years apart
    public FakebookArrayList<SiblingInfo> findPotentialSiblings() throws SQLException {
        FakebookArrayList<SiblingInfo> results = new FakebookArrayList<SiblingInfo>("\n");

        try (Statement stmt = oracle.createStatement(FakebookOracleConstants.AllScroll,
                FakebookOracleConstants.ReadOnly)) {
            ResultSet rst = stmt.executeQuery(
                "SELECT DISTINCT U.USER_ID, U.FIRST_NAME, U.LAST_NAME, H.USER_ID, H.FIRST_NAME " + 
                "FROM " + UsersTable + " U, " + UsersTable + " H, " + HometownCitiesTable + " UH, " + 
                HometownCitiesTable + " HH, " + FriendsTable + " F " +
                "WHERE U.USER_ID < H.USER_ID AND U.LAST_NAME = H.LAST_NAME AND ABS(U.YEAR_OF_BIRTH - H.YEAR_OF_BIRTH) < 10 " +
                "AND UH.USER_ID = U.USER_ID AND HH.USER_ID = H.USER_ID AND UH.HOMETOWN_CITY_ID = HH.HOMETOWN_CITY_ID " +
                "AND U.USER_ID = F.USER1_ID AND H.USER_ID = F.USER2_ID " +
                "ORDER BY U.USER_ID ASC, H.USER_ID ASC");
            while(rst.next()){
                UserInfo u1 = new UserInfo(rst.getInt(1), rst.getString(2), rst.getString(3));
                UserInfo u2 = new UserInfo(rst.getInt(4), rst.getString(5), rst.getString(3));
                SiblingInfo si = new SiblingInfo(u1, u2);
                results.add(si);
            }
        } catch (SQLException e) {
            System.err.println(e.getMessage());
        }

        return results;
    }

    // Member Variables
    private Connection oracle;
    private final String UsersTable = FakebookOracleConstants.UsersTable;
    private final String CitiesTable = FakebookOracleConstants.CitiesTable;
    private final String FriendsTable = FakebookOracleConstants.FriendsTable;
    private final String CurrentCitiesTable = FakebookOracleConstants.CurrentCitiesTable;
    private final String HometownCitiesTable = FakebookOracleConstants.HometownCitiesTable;
    private final String ProgramsTable = FakebookOracleConstants.ProgramsTable;
    private final String EducationTable = FakebookOracleConstants.EducationTable;
    private final String EventsTable = FakebookOracleConstants.EventsTable;
    private final String AlbumsTable = FakebookOracleConstants.AlbumsTable;
    private final String PhotosTable = FakebookOracleConstants.PhotosTable;
    private final String TagsTable = FakebookOracleConstants.TagsTable;
}
