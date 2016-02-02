package com.gft.unity.android.util;

public class FilenameUtils {

	
	/**
     * The Unix separator character.
     */
    public static final char UNIX_SEPARATOR = '/';

    /**
     * The Windows separator character.
     */
    public static final char WINDOWS_SEPARATOR = '\\';

    
	/**
     * Gets the name minus the path from a full filename.
     * <p>
     * This method will handle a file in either Unix or Windows format.
     * The text after the last forward or backslash is returned.
     * <pre>
     * a/b/c.txt --> c.txt
     * a.txt     --> a.txt
     * a/b/c     --> c
     * a/b/c/    --> ""
     * </pre>
     * <p>
     * The output will be the same irrespective of the machine that the code is running on.
     *
     * @param filename  the filename to query, null returns null
     * @return the name of the file without the path, or an empty string if none exists
     */
    public static String getName(final String filename) {
        if (filename == null) {
            return null;
        }
        final int index = indexOfLastSeparator(filename);
        return filename.substring(index + 1);
    }
    
    /**
     * Returns the index of the last directory separator character.
     * <p>
     * This method will handle a file in either Unix or Windows format.
     * The position of the last forward or backslash is returned.
     * <p>
     * The output will be the same irrespective of the machine that the code is running on.
     *
     * @param filename  the filename to find the last path separator in, null returns -1
     * @return the index of the last separator character, or -1 if there
     * is no such character
     */
    public static int indexOfLastSeparator(final String filename) {
        if (filename == null) {
            return -1;
        }
        final int lastUnixPos = filename.lastIndexOf(UNIX_SEPARATOR);
        final int lastWindowsPos = filename.lastIndexOf(WINDOWS_SEPARATOR);
        return Math.max(lastUnixPos, lastWindowsPos);
    }
    
}
